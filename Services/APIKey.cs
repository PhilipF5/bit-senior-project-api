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
			var rngCsp = RandomNumberGenerator.Create();
			byte[] key = new byte[8];
			rngCsp.GetBytes(key);
			string hex = BitConverter.ToString(key).ToLower();
			return hex.Replace("-", "");
		}

		public static int GetBuyerID(string key)
		{
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var command = new NpgsqlCommand("SELECT APIKey, Buyer_ID FROM Users WHERE APIKey = @key", connection);
				command.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
				connection.Open();
				var reader = command.ExecuteReader();
				if (!reader.HasRows)
				{
					return 0;
				}
				reader.Read();
				if ((string)reader[0] == key)
				{
					return (int)reader[1];
				}
				else return 0;
			}
		}

		public static bool IsBuyer(string key)
		{
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var command = new NpgsqlCommand("SELECT APIKey FROM Users WHERE APIKey = @key", connection);
				command.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
				connection.Open();
				var reader = command.ExecuteReader();
				if (!reader.HasRows)
				{
					return false;
				}
				reader.Read();
				if ((string)reader[0] == key)
				{
					return true;
				}
				else return false;
			}
		}

		public static bool IsManager(string key)
		{
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var command = new NpgsqlCommand("SELECT APIKey FROM Admins WHERE APIKey = @key", connection);
				command.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
				connection.Open();
				var reader = command.ExecuteReader();
				if (!reader.HasRows)
				{
					return false;
				}
				reader.Read();
				if ((string)reader[0] == key)
				{
					return true;
				}
				else return false;
			}
		}

		public static bool IsValid(string key)
		{
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var command = new NpgsqlCommand("SELECT APIKey FROM Users WHERE APIKey = @key UNION SELECT APIKey FROM Admins WHERE APIKey = @key", connection);
				command.Parameters.Add("@key", NpgsqlTypes.NpgsqlDbType.Varchar).Value = key;
				connection.Open();
				var reader = command.ExecuteReader();
				if (!reader.HasRows)
				{
					return false;
				}
				reader.Read();
				if ((string)reader[0] == key)
				{
					return true;
				}
				else return false;
			}
		}
	}
}
