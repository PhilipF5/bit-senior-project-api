using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using api.Services;
using System.Runtime.Serialization;
using Npgsql;

namespace api.Models
{
	public class Auction
	{
		public string Address;
		public List<Buyer> Buyers = new List<Buyer>(); // Not serialized for mobile users
		public string City;
		public DateTime EndTime;
		public int ID;
		public List<Lot> Lots = new List<Lot>();
		public int Participants;
		public string PostalCode;
		public DateTime StartTime;
		public string State
		{
			get
			{
				return StateInfo.GetName(StateCode);
			}
		}
		public string StateCode;

		[IgnoreDataMember]
		public bool SerializeBuyers = false;

		public Auction(int id)
		{
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				// Load Auctions table data
				var aCommand = new NpgsqlCommand("SELECT * FROM Auctions WHERE ID = @id", connection);
				aCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
				connection.Open();
				var aReader = aCommand.ExecuteReader();
				aReader.Read();
				ID = (int)aReader[0];
				StartTime = DateTime.Parse(aReader[2].ToString());
				EndTime = DateTime.Parse(aReader[3].ToString());
				var auctionHouseID = (int)aReader[1];
				aReader.Close();

				// Load AuctionHouses table data
				var hCommand = new NpgsqlCommand("SELECT * FROM AuctionHouses WHERE ID = @id", connection);
				hCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = auctionHouseID;
				var hReader = hCommand.ExecuteReader();
				hReader.Read();
				Address = hReader[1].ToString();
				City = hReader[2].ToString();
				StateCode = hReader[3].ToString();
				PostalCode = hReader[4].ToString();
				hReader.Close();

				// Load Participants table data
				var pCommand = new NpgsqlCommand("SELECT Buyer_ID FROM Participants WHERE Auction_ID = @id", connection);
				pCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
				var pReader = pCommand.ExecuteReader();
				while (pReader.Read())
				{
					Buyers.Add(new Buyer((int)pReader[0]));
				}
				Participants = Buyers.Count;
				pReader.Close();

				// Load Lots table data
				var lCommand = new NpgsqlCommand("SELECT ID FROM Lots WHERE Auction_ID = @id", connection);
				lCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
				var lReader = lCommand.ExecuteReader();
				while (lReader.Read())
				{
					Lots.Add(new Lot((int)lReader[0]));
				}
			}
		}

		public static List<Auction> GetAll(bool loadBuyers = false)
		{
			List<Auction> auctions = new List<Auction>();
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var aCommand = new NpgsqlCommand("SELECT ID FROM Auctions ORDER BY StartTime", connection);
				connection.Open();
				var aReader = aCommand.ExecuteReader();
				while (aReader.Read())
				{
					auctions.Add(new Auction((int)aReader[0]));
				}
				foreach (Auction auct in auctions)
				{
					auct.SerializeBuyers = loadBuyers;
				}
			}
			return auctions;
		}

		public bool ShouldSerializeBuyers()
		{
			return SerializeBuyers;
		}
	}
}
