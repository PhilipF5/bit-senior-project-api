using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using api.Services;
using Npgsql;
using api.Controllers;

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
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var cCommand = new NpgsqlCommand("SELECT * FROM CreditAccounts WHERE ID = @id", connection);
				cCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
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
				cReader.Close();

				var bCommand = new NpgsqlCommand("SELECT Bids.Amount, Bids.Status FROM Bids, Participants, Buyers WHERE Buyers.Account_ID = @id AND Bids.Participant_ID = Participants.ID AND Participants.Buyer_ID = Buyers.ID", connection);
				bCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
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
				bReader.Close();

				var buyersCommand = new NpgsqlCommand("SELECT ID FROM Buyers WHERE Account_ID = @id", connection);
				buyersCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
				var buyersReader = buyersCommand.ExecuteReader();
				while (buyersReader.Read())
				{
					Buyers.Add(new Buyer((int)buyersReader[0]));
				}
			}
		}

		public static Account Create(InputAccount input)
		{
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var cCommand = new NpgsqlCommand("INSERT INTO CreditAccounts(Owner, Credit, ContactEmail, ContactPhone, Address, City, State, PostalCode) VALUES (@owner, @credit, @email, @phone, @address, @city, @state, @postal)", connection);
				connection.Open();
				cCommand.Parameters.Add("@owner", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.DealerName;
				cCommand.Parameters.Add("@credit", NpgsqlTypes.NpgsqlDbType.Money).Value = input.CreditAmount;
				cCommand.Parameters.Add("@email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.ContactEmail;
				cCommand.Parameters.Add("@phone", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.ContactPhone;
				cCommand.Parameters.Add("@address", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.StreetAddress;
				cCommand.Parameters.Add("@city", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.City;
				cCommand.Parameters.Add("@state", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.State;
				cCommand.Parameters.Add("@postal", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.PostalCode;

				cCommand.ExecuteNonQuery();
				var getID = new NpgsqlCommand("SELECT CAST(CURRVAL(pg_get_serial_sequence('creditaccounts','id')) AS INTEGER)", connection).ExecuteReader();
				getID.Read();
				return new Account((int)getID[0]);
			}
		}

		public static List<Account> GetAll()
		{
			List<Account> accounts = new List<Account>();
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var aCommand = new NpgsqlCommand("SELECT ID FROM CreditAccounts", connection);
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
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var uCommand = new NpgsqlCommand("SELECT Account_ID FROM Users, Buyers WHERE APIKey = @key AND Users.Buyer_ID = Buyers.ID", connection);
				uCommand.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
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
