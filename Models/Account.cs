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
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand cCommand = new NpgsqlCommand("SELECT * FROM CreditAccounts WHERE ID = @id", connection);
			cCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
			NpgsqlDataReader cReader = cCommand.ExecuteReader();
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

			NpgsqlCommand bCommand = new NpgsqlCommand("SELECT Bids.Amount, Bids.Status FROM Bids, Participants, Buyers WHERE Buyers.Account_ID = @id AND Bids.Participant_ID = Participants.ID AND Participants.Buyer_ID = Buyers.ID", connection);
			bCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
			NpgsqlDataReader bReader = bCommand.ExecuteReader();
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

			NpgsqlCommand buyersCommand = new NpgsqlCommand("SELECT ID FROM Buyers WHERE Account_ID = @id", connection);
			buyersCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
			NpgsqlDataReader buyersReader = buyersCommand.ExecuteReader();
			while (buyersReader.Read())
			{
				Buyers.Add(new Buyer((int)buyersReader[0]));
			}
			buyersReader.Close();

			connection.Close();
		}

		public static Account Create(InputAccount input)
		{
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand cCommand = new NpgsqlCommand("INSERT INTO CreditAccounts(Owner, Credit, ContactEmail, ContactPhone, Address, City, State, PostalCode) VALUES (@owner, @credit, @email, @phone, @address, @city, @state, @postal)", connection);
			cCommand.Parameters.Add("@owner", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.DealerName;
			cCommand.Parameters.Add("@credit", NpgsqlTypes.NpgsqlDbType.Money).Value = input.CreditAmount;
			cCommand.Parameters.Add("@email", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.ContactEmail;
			cCommand.Parameters.Add("@phone", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.ContactPhone;
			cCommand.Parameters.Add("@address", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.StreetAddress;
			cCommand.Parameters.Add("@city", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.City;
			cCommand.Parameters.Add("@state", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.State;
			cCommand.Parameters.Add("@postal", NpgsqlTypes.NpgsqlDbType.Varchar).Value = input.PostalCode;
			cCommand.ExecuteNonQuery();

			NpgsqlDataReader getIDReader = new NpgsqlCommand("SELECT CAST(CURRVAL(pg_get_serial_sequence('creditaccounts','id')) AS INTEGER)", connection).ExecuteReader();
			getIDReader.Read();
			int accountID = (int)getIDReader[0];
			getIDReader.Close();

			connection.Close();

			return new Account(accountID);
		}

		public static List<Account> GetAll()
		{
			List<Account> accounts = new List<Account>();
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand aCommand = new NpgsqlCommand("SELECT ID FROM CreditAccounts", connection);
			NpgsqlDataReader aReader = aCommand.ExecuteReader();
			while (aReader.Read())
			{
				accounts.Add(new Account((int)aReader[0]));
			}
			foreach (Account acct in accounts)
			{
				acct.SerializeBuyers = true;
			}
			aReader.Close();

			connection.Close();

			return accounts;
		}

		public static Account GetFromKey(string key)
		{
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand uCommand = new NpgsqlCommand("SELECT Account_ID FROM Users, Buyers WHERE APIKey = @key AND Users.Buyer_ID = Buyers.ID", connection);
			uCommand.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
			NpgsqlDataReader uReader = uCommand.ExecuteReader();
			uReader.Read();
			int accountID = (int)uReader[0];
			uReader.Close();

			connection.Close();

			return new Account(accountID);
		}

		public bool ShouldSerializeBuyers()
		{
			return SerializeBuyers;
		}
	}
}
