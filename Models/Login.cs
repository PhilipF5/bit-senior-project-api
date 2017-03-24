using System;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using api.Models;

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
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				NpgsqlCommand command;
				command = new NpgsqlCommand("SELECT APIKey, FirstName, LastName, Username, Password FROM Admins, Managers WHERE Username = @user AND Admins.Manager_ID = Managers.ID UNION SELECT APIKey, FirstName, LastName, Username, AuthKey FROM Users, Buyers WHERE Username = @user AND Users.Buyer_ID = Buyers.ID", connection);
				command.Parameters.Add("@user", NpgsqlTypes.NpgsqlDbType.Varchar).Value = user;
				connection.Open();
				var reader = command.ExecuteReader();
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
			}
		}
	}
}
