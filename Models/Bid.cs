using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Internal;
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
		public string Status; // "Placed" "Winner" "Outbid" "Low" "Late" "Duplicate" or "Unauthorized"

		public Bid(string status = "Unauthorized")
		{
			Status = status;
		}

		public Bid(int id)
		{
			using (var connection = new SqlConnection(Environment.GetEnvironmentVariable("SQLAZURECONNSTR_bit4454database")))
			{
				var bidsCommand = new SqlCommand("SELECT * FROM Bids WHERE ID = @id", connection);
				bidsCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
				connection.Open();
				var bidsReader = bidsCommand.ExecuteReader();
				bidsReader.Read();
				ID = (int)bidsReader[0];
				LotID = (int)bidsReader[1];
				Amount = (decimal)bidsReader[3];
				BidTime = DateTime.Parse(bidsReader[4].ToString());
				Status = bidsReader[5].ToString();

				var pCommand = new SqlCommand("SELECT Buyer_ID, Account_ID FROM Participants, Buyers WHERE Participants.ID = @id AND Buyer_ID = Buyers.ID", connection);
				pCommand.Parameters.Add("@id", SqlDbType.Int).Value = (int)bidsReader[2];
				var pReader = pCommand.ExecuteReader();
				pReader.Read();
				BuyerID = (int)pReader[0];
				AccountID = (int)pReader[1];
			}
		}

		public static Bid Place(string key, int lotID, decimal amount)
		{
			using (var connection = new SqlConnection(Environment.GetEnvironmentVariable("SQLAZURECONNSTR_bit4454database")))
			{
				/*
				var lCommand = new SqlCommand("SELECT Auction_ID, MinPrice FROM Lots WHERE ID = @id", connection);
				lCommand.Parameters.Add("@id", SqlDbType.Int).Value = lotID;
				connection.Open();
				var lReader = lCommand.ExecuteReader();
				lReader.Read();
				var auctionID = (int)lReader[0];
				var minPrice = (int)lReader[1];
				*/

				var lot = new Lot(lotID);
				string status;

				var buyerCommand = new SqlCommand("SELECT Buyers.ID FROM Users, Buyers WHERE APIKey = @key AND Users.Buyer_ID = Buyers.ID", connection);
				buyerCommand.Parameters.Add("@key", SqlDbType.VarChar).Value = key;
				connection.Open();
				var buyerReader = buyerCommand.ExecuteReader();
				buyerReader.Read();
				var buyer = new Buyer((int)buyerReader[0]);

				var pCommand = new SqlCommand("SELECT ID FROM Participants WHERE Buyer_ID = @buyer AND Auction_ID = @auction", connection);
				pCommand.Parameters.Add("@buyer", SqlDbType.Int).Value = buyer.ID;
				pCommand.Parameters.Add("@auction", SqlDbType.Int).Value = lot.AuctionID;
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

				if (lot.BidsMax == null)
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

				var bidInsertCommand = new SqlCommand("INSERT INTO Bids(Lot_ID, Participant_ID, Amount, BidTime, Status) VALUES (@lotID, @pID, @amount, @bidTime, @status)", connection);
				bidInsertCommand.Parameters.Add("@lotID", SqlDbType.Int).Value = lot.ID;
				bidInsertCommand.Parameters.Add("@pID", SqlDbType.Int).Value = pID;
				bidInsertCommand.Parameters.Add("@amount", SqlDbType.Decimal).Value = amount;
				bidInsertCommand.Parameters.Add("@bidTime", SqlDbType.DateTime).Value = DateTime.UtcNow;
				bidInsertCommand.Parameters.Add("@status", SqlDbType.VarChar).Value = status;
				if (lot.BidsMax != null && status == "Placed")
				{
					var bidUpdateCommand = new SqlCommand("UPDATE Bids SET Status = 'Outbid' WHERE ID = @outbid", connection);
					bidUpdateCommand.Parameters.Add("@outbid", SqlDbType.Int).Value = lot.BidsMax.ID;
					bidUpdateCommand.ExecuteNonQuery();
				}
				bidInsertCommand.ExecuteNonQuery();

				var getID = new SqlCommand("SELECT CONVERT(int,IDENT_CURRENT('Bids'))", connection).ExecuteReader();
				getID.Read();
				return new Bid((int)getID[0]);
			}
		}
	}
}
