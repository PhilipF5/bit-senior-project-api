using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Npgsql;

namespace api.Services
{
	public static class APIKey
	{
		public static string Generate()
		{
			RandomNumberGenerator rngCsp = RandomNumberGenerator.Create();
			byte[] key = new byte[8];
			rngCsp.GetBytes(key);
			string hex = BitConverter.ToString(key).ToLower();
			return hex.Replace("-", "");
		}

		public static int GetBuyerID(string key)
		{
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand command = new NpgsqlCommand("SELECT APIKey, Buyer_ID FROM Users WHERE APIKey = @key", connection);
			command.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
			NpgsqlDataReader reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				return 0;
			}
			reader.Read();
			string check = (string)reader[0];
			int buyerID = (int)reader[1];
			reader.Close();
			connection.Close();
			if (check == key)
			{
				return buyerID;
			}
			else return 0;
		}

		public static bool IsBuyer(string key)
		{
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand command = new NpgsqlCommand("SELECT APIKey FROM Users WHERE APIKey = @key", connection);
			command.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
			NpgsqlDataReader reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				return false;
			}
			reader.Read();
			string check = (string)reader[0];
			reader.Close();
			connection.Close();
			if (check == key)
			{
				return true;
			}
			else return false;
		}

		public static bool IsManager(string key)
		{
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand command = new NpgsqlCommand("SELECT APIKey FROM Admins WHERE APIKey = @key", connection);
			command.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
			NpgsqlDataReader reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				return false;
			}
			reader.Read();
			string check = (string)reader[0];
			reader.Close();
			connection.Close();
			if (check == key)
			{
				return true;
			}
			else return false;
		}

		public static bool IsValid(string key)
		{
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand command = new NpgsqlCommand("SELECT APIKey FROM Users WHERE APIKey = @key UNION SELECT APIKey FROM Admins WHERE APIKey = @key", connection);
			command.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
			NpgsqlDataReader reader = command.ExecuteReader();
			if (!reader.HasRows)
			{
				return false;
			}
			reader.Read();
			string check = (string)reader[0];
			reader.Close();
			connection.Close();
			if (check == key)
			{
				return true;
			}
			else return false;
		}
	}
}
