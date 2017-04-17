using System;
using Npgsql;

namespace api.Services
{
	public class Database
	{
		public NpgsqlConnection Connection;

		public Database()
		{
			Connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres"));
			Connection.Open();
		}
	}
}
