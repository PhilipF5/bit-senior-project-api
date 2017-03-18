using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using api.Services;
namespace api.Models
{
	public class Account
	{
		public string Address;
		public decimal AvailableCredit
		{
			get
			{
				return TotalCredit - UsedCredit;
			}
		}
		public List<Buyer> Buyers = new List<Buyer>(); // Not serialized for mobile users
		public string City;
		public string ContactEmail;
		public string ContactPhone;
		public int ID;
		public string Owner;
		public string PostalCode;
		public string State
		{
			get
			{
				return StateInfo.GetName(StateCode);
			}
		}
		public string StateCode;
		public decimal TotalCredit;
		public decimal TotalSpent = 0;
		public decimal UsedCredit = 0;

		[IgnoreDataMember]
		public bool SerializeBuyers = false;

		public Account(int id)
		{
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var cCommand = new SqlCommand("SELECT * FROM CreditAccounts WHERE ID = @id", connection);
				cCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
				connection.Open();
				var cReader = cCommand.ExecuteReader();
				cReader.Read();
				ID = (int)cReader[0];
				Owner = (string)cReader[1];
				TotalCredit = (decimal)cReader[2];
				ContactEmail = (string)cReader[3];
				ContactPhone = (string)cReader[4];
				Address = (string)cReader[5];
				City = (string)cReader[6];
				StateCode = (string)cReader[7];
				PostalCode = (string)cReader[8];

				var bCommand = new SqlCommand("SELECT Bids.Amount, Bids.Status FROM Bids, Participants, Buyers WHERE Buyers.Account_ID = @id AND Bids.Participant_ID = Participants.ID AND Participants.Buyer_ID = Buyers.ID", connection);
				bCommand.Parameters.Add("@id", SqlDbType.Int).Value = ID;
				var bReader = bCommand.ExecuteReader();
				while (bReader.Read())
				{
					if ((string)bReader[1] == "Placed")
					{
						UsedCredit += (decimal)bReader[0];
					}
					else if ((string)bReader[1] == "Winner")
					{
						TotalSpent += (decimal)bReader[0];
					}
				}

				var buyersCommand = new SqlCommand("SELECT ID FROM Buyers WHERE Account_ID = @id", connection);
				buyersCommand.Parameters.Add("@id", SqlDbType.Int).Value = ID;
				var buyersReader = buyersCommand.ExecuteReader();
				while (buyersReader.Read())
				{
					Buyers.Add(new Buyer((int)buyersReader[0]));
				}
			}
		}

		public static List<Account> GetAll()
		{
			List<Account> accounts = new List<Account>();
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var aCommand = new SqlCommand("SELECT ID FROM CreditAccounts", connection);
				connection.Open();
				var aReader = aCommand.ExecuteReader();
				while (aReader.Read())
				{
					accounts.Add(new Account((int)aReader[0]));
				}
				foreach (Account acct in accounts)
				{
					acct.SerializeBuyers = true;
				}
			}
			return accounts;
		}

		public static Account GetFromKey(string key)
		{
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var uCommand = new SqlCommand("SELECT Account_ID FROM Users, Buyers WHERE APIKey = @key AND Users.Buyer_ID = Buyers.ID", connection);
				uCommand.Parameters.Add("@key", SqlDbType.VarChar).Value = key;
				connection.Open();
				var uReader = uCommand.ExecuteReader();
				uReader.Read();
				return new Account((int)uReader[0]);
			}
		}

		public bool ShouldSerializeBuyers()
		{
			return SerializeBuyers;
		}
	}
}
