using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Internal;
using Npgsql;

namespace api.Models
{
	public class Bid
	{
		public int AccountID;
		public decimal Amount;
		public DateTime BidTime;
		public int BuyerID;
		public int ID;
		public int LotID;
		public string Status; // "Placed" "Winner" "Outbid" "Low" "Late" "Duplicate" "Unauthorized" or "Bounced"

		public Bid(string status = "Unauthorized")
		{
			Status = status;
		}

		public Bid(int id)
		{
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var bidsCommand = new NpgsqlCommand("SELECT * FROM Bids WHERE ID = @id", connection);
				bidsCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
				connection.Open();
				var bidsReader = bidsCommand.ExecuteReader();
				bidsReader.Read();
				ID = (int)bidsReader[0];
				LotID = (int)bidsReader[1];
				Amount = (decimal)bidsReader[3];
				BidTime = DateTime.Parse(bidsReader[4].ToString());
				Status = bidsReader[5].ToString();

				var pCommand = new NpgsqlCommand("SELECT Buyer_ID, Account_ID FROM Participants, Buyers WHERE Participants.ID = @id AND Buyer_ID = Buyers.ID", connection);
				pCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = (int)bidsReader[2];
				var pReader = pCommand.ExecuteReader();
				pReader.Read();
				BuyerID = (int)pReader[0];
				AccountID = (int)pReader[1];
			}
		}

		public static Bid Place(string key, int lotID, decimal amount)
		{
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var lot = new Lot(lotID);
				string status;

				var buyerCommand = new NpgsqlCommand("SELECT Buyers.ID, Buyers.Account_ID FROM Users, Buyers WHERE APIKey = @key AND Users.Buyer_ID = Buyers.ID", connection);
				buyerCommand.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
				connection.Open();
				var buyerReader = buyerCommand.ExecuteReader();
				buyerReader.Read();
				var buyer = new Buyer((int)buyerReader[0]);

				var pCommand = new NpgsqlCommand("SELECT ID FROM Participants WHERE Buyer_ID = @buyer AND Auction_ID = @auction", connection);
				pCommand.Parameters.Add("@buyer", NpgsqlTypes.NpgsqlDbType.Integer).Value = buyer.ID;
				pCommand.Parameters.Add("@auction", NpgsqlTypes.NpgsqlDbType.Integer).Value = lot.AuctionID;
				var pReader = pCommand.ExecuteReader();
				pReader.Read();
				int pID;
				if (!pReader.HasRows)
				{
					return new Bid();
				}
				else
				{
					pID = (int)pReader[0];
				}

				if (amount > new Account((int)buyerReader[1]).AvailableCredit)
				{
					status = "Bounced";
				}
				else if (amount < lot.MinPrice)
				{
					status = "Low";
				}
				else if (lot.BidsMax == null)
				{
					status = "Placed";
				}
				else if (lot.Winner != null)
				{
					status = "Late";
				}
				else if (buyer.AccountID == lot.BidsMax.AccountID)
				{
					status = "Duplicate";
				}
				else if (amount <= lot.BidsMax.Amount)
				{
					status = "Low";
				}
				else
				{
					status = "Placed";
				}

				var bidInsertCommand = new NpgsqlCommand("INSERT INTO Bids(Lot_ID, Participant_ID, Amount, BidTime, Status) VALUES (@lotID, @pID, @amount, @bidTime, @status)", connection);
				bidInsertCommand.Parameters.Add("@lotID", NpgsqlTypes.NpgsqlDbType.Integer).Value = lot.ID;
				bidInsertCommand.Parameters.Add("@pID", NpgsqlTypes.NpgsqlDbType.Integer).Value = pID;
				bidInsertCommand.Parameters.Add("@amount", NpgsqlTypes.NpgsqlDbType.Money).Value = amount;
				bidInsertCommand.Parameters.Add("@bidTime", NpgsqlTypes.NpgsqlDbType.TimestampTZ).Value = DateTime.UtcNow;
				bidInsertCommand.Parameters.Add("@status", NpgsqlTypes.NpgsqlDbType.Varchar).Value = status;
				if (lot.BidsMax != null && status == "Placed")
				{
					var bidUpdateCommand = new NpgsqlCommand("UPDATE Bids SET Status = 'Outbid' WHERE ID = @outbid", connection);
					bidUpdateCommand.Parameters.Add("@outbid", NpgsqlTypes.NpgsqlDbType.Integer).Value = lot.BidsMax.ID;
					bidUpdateCommand.ExecuteNonQuery();
				}
				bidInsertCommand.ExecuteNonQuery();

				var getID = new NpgsqlCommand("SELECT CONVERT(int,IDENT_CURRENT('Bids'))", connection).ExecuteReader();
				getID.Read();
				return new Bid((int)getID[0]);
			}
		}
	}
}
