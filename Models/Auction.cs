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
			NpgsqlConnection connection = new Database().Connection;

			// Load Auctions table data
			NpgsqlCommand aCommand = new NpgsqlCommand("SELECT * FROM Auctions WHERE ID = @id", connection);
			aCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
			NpgsqlDataReader aReader = aCommand.ExecuteReader();
			aReader.Read();
			ID = (int)aReader[0];
			StartTime = DateTime.Parse(aReader[2].ToString()).ToUniversalTime();
			EndTime = DateTime.Parse(aReader[3].ToString()).ToUniversalTime();
			int auctionHouseID = (int)aReader[1];
			aReader.Close();

			// Load AuctionHouses table data
			NpgsqlCommand hCommand = new NpgsqlCommand("SELECT * FROM AuctionHouses WHERE ID = @id", connection);
			hCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = auctionHouseID;
			NpgsqlDataReader hReader = hCommand.ExecuteReader();
			hReader.Read();
			Address = hReader[1].ToString();
			City = hReader[2].ToString();
			StateCode = hReader[3].ToString();
			PostalCode = hReader[4].ToString();
			hReader.Close();

			// Load Participants table data
			NpgsqlCommand pCommand = new NpgsqlCommand("SELECT Buyer_ID FROM Participants WHERE Auction_ID = @id", connection);
			pCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
			NpgsqlDataReader pReader = pCommand.ExecuteReader();
			while (pReader.Read())
			{
				Buyers.Add(new Buyer((int)pReader[0]));
			}
			Participants = Buyers.Count;
			pReader.Close();

			// Load Lots table data
			NpgsqlCommand lCommand = new NpgsqlCommand("SELECT ID FROM Lots WHERE Auction_ID = @id", connection);
			lCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
			NpgsqlDataReader lReader = lCommand.ExecuteReader();
			while (lReader.Read())
			{
				Lots.Add(new Lot((int)lReader[0]));
			}
			lReader.Close();

			connection.Close();
		}

		public static List<Auction> GetAll(bool loadBuyers = false)
		{
			List<Auction> auctions = new List<Auction>();
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand aCommand = new NpgsqlCommand("SELECT ID FROM Auctions ORDER BY StartTime", connection);
			NpgsqlDataReader aReader = aCommand.ExecuteReader();
			while (aReader.Read())
			{
				auctions.Add(new Auction((int)aReader[0]));
			}
			foreach (Auction auct in auctions)
			{
				auct.SerializeBuyers = loadBuyers;
			}
			aReader.Close();

			connection.Close();

			return auctions;
		}

		public bool ShouldSerializeBuyers()
		{
			return SerializeBuyers;
		}
	}
}
