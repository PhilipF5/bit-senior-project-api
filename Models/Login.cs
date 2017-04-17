using System;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using api.Models;
using api.Services;

namespace api
{
	public class Login
	{
		public string APIKey;
		public string Error;
		public string FirstName;
		public string LastName;
		public string Role;
		public string Username;

		public Login(string user, string pass)
		{
			NpgsqlConnection connection = new Database().Connection;

			user = user.ToLower();
			NpgsqlCommand command = new NpgsqlCommand("SELECT APIKey, FirstName, LastName, LOWER(Username), Password FROM Admins, Managers WHERE LOWER(Username) = @user AND Admins.Manager_ID = Managers.ID UNION SELECT APIKey, FirstName, LastName, LOWER(Username), AuthKey FROM Users, Buyers WHERE LOWER(Username) = @user AND Users.Buyer_ID = Buyers.ID", connection);
			command.Parameters.Add("@user", NpgsqlTypes.NpgsqlDbType.Varchar).Value = user;
			NpgsqlDataReader reader = command.ExecuteReader();
			reader.Read();
			if (reader.HasRows)
			{
				if ((string)reader[3] == user)
				{
					if ((string)reader[4] == pass)
					{
						APIKey = (string)reader[0];
						FirstName = (string)reader[1];
						LastName = (string)reader[2];
						Username = (string)reader[3];
						if (Services.APIKey.IsBuyer(APIKey))
						{
							Role = "user";
						}
						else if (Services.APIKey.IsManager(APIKey))
						{
							Role = "admin";
						}
					}
					else
					{
						Username = user;
						Error = "Invalid password";
					}
				}
				else
				{
					Username = user;
					Error = "Invalid username";
				}
			}
			else
			{
				Username = user;
				Error = "Invalid username";
			}
			reader.Close();
			connection.Close();
		}
	}
}
