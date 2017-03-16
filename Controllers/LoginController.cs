using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class LoginController : Controller
	{
		[HttpPost("mobile")]
		public string Mobile([FromBody] string[] creds)
		{
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var command = new SqlCommand("SELECT * FROM Users WHERE Username = @user", connection);
				command.Parameters.Add("@user", SqlDbType.VarChar).Value = creds[0];
				connection.Open();
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						if (reader[2].ToString() == creds[1])
						{
							return reader[4].ToString();
						}
						else
						{
							return "Invalid password";
						}
					}
				}
			}
			return "Invalid username";
		}

		[HttpPost("web")]
		public string Web([FromBody] string[] creds)
		{
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var command = new SqlCommand("SELECT * FROM Admins WHERE Username = @user", connection);
				command.Parameters.Add("@user", SqlDbType.VarChar).Value = creds[0];
				connection.Open();
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						if (reader[2].ToString() == creds[1])
						{
							return reader[4].ToString();
						}
						else
						{
							return "Invalid password";
						}
					}
				}
			}
			return "Invalid username";
		}
	}
}
