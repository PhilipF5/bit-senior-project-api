using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

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

		public static bool IsBuyer(string key)
		{
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var command = new SqlCommand("SELECT APIKey FROM Users WHERE APIKey = @key", connection);
				command.Parameters.Add("@key", SqlDbType.VarChar).Value = key;
				connection.Open();
				var reader = command.ExecuteReader();
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
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var command = new SqlCommand("SELECT APIKey FROM Admins WHERE APIKey = @key", connection);
				command.Parameters.Add("@key", SqlDbType.VarChar).Value = key;
				connection.Open();
				var reader = command.ExecuteReader();
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
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var command = new SqlCommand("SELECT APIKey FROM Users WHERE APIKey = @key UNION SELECT APIKey FROM Admins WHERE APIKey = @key", connection);
				command.Parameters.Add("@key", SqlDbType.VarChar).Value = key;
				connection.Open();
				var reader = command.ExecuteReader();
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
