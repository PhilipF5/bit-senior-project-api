using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using api.Controllers;
using api.Services;

namespace api.Models
{
	public class Buyer
	{
		public int AccountID;
		public List<int> Auctions = new List<int>();
		public int AuctionCount
		{
			get
			{
				return ParticipantID.Count;
			}
		}
		public List<Bid> Bids = new List<Bid>();
		public int BidsCount
		{
			get
			{
				return Bids.Count;
			}
		}
		public decimal BidsMax = 0;
		public decimal BidsMin = 100000;
		public int ID; // Buyers table
		public string FirstName;
		public string FullName
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}
		public string LastName;
		private List<int> ParticipantID = new List<int>();
		public decimal TotalSpent = 0;
		public string Username;

		public Buyer(int id)
		{
			NpgsqlConnection connection = new Database().Connection;

			// Load Buyers table data
			NpgsqlCommand buyersCommand = new NpgsqlCommand("SELECT * FROM Buyers WHERE ID = @id", connection);
			buyersCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
			NpgsqlDataReader buyersReader = buyersCommand.ExecuteReader();
			buyersReader.Read();
			ID = (int)buyersReader[0];
			FirstName = (string)buyersReader[1];
			LastName = (string)buyersReader[2];
			AccountID = (int)buyersReader[3];
			buyersReader.Close();

			// Load Auctions table data
			NpgsqlCommand auctionsCommand = new NpgsqlCommand("SELECT ID, Auction_ID FROM Participants WHERE Buyer_ID = @id", connection);
			auctionsCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
			NpgsqlDataReader auctionsReader = auctionsCommand.ExecuteReader();
			while (auctionsReader.Read())
			{
				ParticipantID.Add((int)auctionsReader[0]);
				Auctions.Add((int)auctionsReader[1]);
			}
			auctionsReader.Close();

			// Load Users table data
			NpgsqlCommand usersCommand = new NpgsqlCommand("SELECT Username FROM Users WHERE Buyer_ID = @id", connection);
			usersCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
			NpgsqlDataReader usersReader = usersCommand.ExecuteReader();
			usersReader.Read();
			Username = (string)usersReader[0];
			usersReader.Close();

			// Load Bids table data
			if (ParticipantID.Count > 0)
			{
				NpgsqlCommand bidsCommand = new NpgsqlCommand("SELECT * FROM Bids WHERE Participant_ID IN ({Range})", connection);
				bidsCommand.AddArrayParameters(ParticipantID.ToArray(), "Range");
				NpgsqlDataReader bidsReader = bidsCommand.ExecuteReader();
				while (bidsReader.Read())
				{
					Bids.Add(new Bid((int)bidsReader[0]));
				}
				if (BidsCount > 0)
				{
					foreach (Bid bid in Bids)
					{
						if (bid.Amount > BidsMax)
						{
							BidsMax = bid.Amount;
						}
						if (bid.Amount < BidsMin)
						{
							BidsMin = bid.Amount;
						}
						if (bid.Status == "Winner")
						{
							TotalSpent += bid.Amount;
						}
					}
				}
				else
				{
					BidsMin = 0;
				}
				bidsReader.Close();
			}

			connection.Close();
		}

		public static Tuple<string, string> Create(InputBuyer input)
		{
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand bCommand = new NpgsqlCommand("INSERT INTO Buyers(FirstName, LastName, Account_ID) VALUES (@first, @last, @acct)", connection);
			bCommand.Parameters.Add("@first", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.FirstName;
			bCommand.Parameters.Add("@last", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.LastName;
			bCommand.Parameters.Add("@acct", NpgsqlTypes.NpgsqlDbType.Integer).Value = input.AccountID;
			bCommand.ExecuteNonQuery();

			NpgsqlDataReader getID = new NpgsqlCommand("SELECT CAST(CURRVAL(pg_get_serial_sequence('buyers','id')) AS INTEGER)", connection).ExecuteReader();
			getID.Read();
			int buyerID = (int)getID[0];
			getID.Close();

			string authKey;
			NpgsqlCommand uCommand = new NpgsqlCommand("INSERT INTO Users(Username, AuthKey, Buyer_ID, APIKey) VALUES (@username, @authkey, @buyer, @apikey)", connection);
			uCommand.Parameters.Add("@username", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.Username;
			uCommand.Parameters.Add("@authkey", NpgsqlTypes.NpgsqlDbType.Varchar).Value = authKey = APIKey.Generate().Substring(3, 4);
			uCommand.Parameters.Add("@buyer", NpgsqlTypes.NpgsqlDbType.Integer).Value = buyerID;
			uCommand.Parameters.Add("@apikey", NpgsqlTypes.NpgsqlDbType.Varchar).Value = APIKey.Generate();
			uCommand.ExecuteNonQuery();

			connection.Close();

			return Tuple.Create(input.Username, authKey);
		}
	}
}
