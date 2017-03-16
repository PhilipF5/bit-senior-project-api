using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
		public List<Buyer> Buyers; // Not serialized for mobile users
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
			}
		}

		public bool ShouldSerializeBuyers()
		{
			return SerializeBuyers;
		}
	}
}
