using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
namespace api.Models
{
	public class Lot
	{
		public int AuctionID;
		public List<Bid> Bids = new List<Bid>();
		public Bid BidsMax
		{
			get
			{
				Bid max = Bids[0];
				foreach (Bid bid in Bids)
				{
					if (bid.Amount > max.Amount)
					{
						max = bid;
					}
				}
				return max;
			}
		}
		public string Color;
		public string Desc // Year + Make + Model + Trim
		{
			get
			{
				return string.Join(" ", new string[] { Year.ToString(), Make, Model, Trim });
			}
		}
		public string DetailLink;
		public int ID;
		public string Make;
		public int MinPrice;
		public string Model;
		public string Status
		{
			get
			{
				if (Winner == null)
				{
					return "Unsold";
				}
				else return "Sold";
			}
		}
		public string Trim;
		public Bid Winner
		{
			get
			{
				if (BidsMax.Status == "Winner")
				{
					return BidsMax;
				}
				else return null;
			}
		}
		public int VehicleID;
		public string VIN;
		public int Year;

		public Lot(int id)
		{
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var vCommand = new SqlCommand("SELECT * FROM Lots, Vehicles, VehicleModels, VehicleMakes WHERE Lots.ID = @id AND Lots.Vehicle_ID = Vehicles.ID AND Vehicles.Model_ID = VehicleModels.ID AND VehicleModels.Make_ID = VehicleMakes.ID", connection);
				vCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
				connection.Open();
				var vReader = vCommand.ExecuteReader();
				vReader.Read();
				AuctionID = (int)vReader[1];
				Color = (string)vReader[8];
				DetailLink = (string)vReader[11];
				ID = (int)vReader[0];
				Make = (string)vReader[17];
				MinPrice = (int)vReader[3];
				Model = (string)vReader[14];
				Trim = (string)vReader[9];
				VehicleID = (int)vReader[2];
				VIN = (string)vReader[5];
				Year = (int)vReader[7];

				var bCommand = new SqlCommand("SELECT ID FROM Bids WHERE Lot_ID = @id", connection);
				bCommand.Parameters.Add("@id", SqlDbType.Int).Value = ID;
				var bReader = bCommand.ExecuteReader();
				while (bReader.Read())
				{
					Bids.Add(new Bid((int)bReader[0]));
				}
			}
		}
	}
}
