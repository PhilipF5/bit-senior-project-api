using System;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using api.Services;
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
		public string State;
		public string StateCode;

		public bool SerializeBuyers = false;

		public Auction(int id)
		{
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				// Load Auctions table data
				var aCommand = new SqlCommand("SELECT * FROM Auctions WHERE ID = @id", connection);
				aCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
				connection.Open();
				var aReader = aCommand.ExecuteReader();
				aReader.Read();
				ID = (int)aReader[0];
				StartTime = DateTime.Parse(aReader[2].ToString());
				EndTime = DateTime.Parse(aReader[3].ToString());

				// Load AuctionHouses table data
				var hCommand = new SqlCommand("SELECT * FROM AuctionHouses WHERE ID = @id", connection);
				hCommand.Parameters.Add("@id", SqlDbType.Int).Value = (int)aReader[1];
				var hReader = hCommand.ExecuteReader();
				hReader.Read();
				Address = hReader[1].ToString();
				City = hReader[2].ToString();
				State = hReader[3].ToString();
				StateCode = StateInfo.GetCode(State);
				PostalCode = hReader[4].ToString();

				// Load Participants table data
				var pCommand = new SqlCommand("SELECT Buyer_ID FROM Participants WHERE Auction_ID = @id", connection);
				pCommand.Parameters.Add("@id", SqlDbType.Int).Value = ID;
				var pReader = pCommand.ExecuteReader();
				while (pReader.Read())
				{
					Buyers.Add(new Buyer((int)pReader[0]));
				}
				Participants = Buyers.Count;

				// Load Lots table data
				var lCommand = new SqlCommand("SELECT Lot_ID FROM Lots WHERE Auction_ID = @id", connection);
				lCommand.Parameters.Add("@id", SqlDbType.Int).Value = ID;
				var lReader = lCommand.ExecuteReader();
				while (lReader.Read())
				{
					Lots.Add(new Lot((int)pReader[0]));
				}
			}
		}

		public bool ShouldSerializeBuyers()
		{
			return SerializeBuyers;
		}
	}
}
