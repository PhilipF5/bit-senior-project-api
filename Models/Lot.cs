using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using Npgsql;
using api.Services;

namespace api.Models
{
	public class Lot
	{
		public int AuctionID;
		public List<Bid> Bids = new List<Bid>();
		public int BidsCount
		{
			get
			{
				return Bids.Count;
			}
		}
		public Bid BidsMax
		{
			get
			{
				if (Bids.Count == 0)
				{
					return null;
				}
				Bid max = null;
				foreach (Bid bid in Bids)
				{
					if (bid.Status == "Placed" || bid.Status == "Winner")
					{
						max = bid;
					}
				}
				return max;
			}
		}
		public string Color;
		public decimal CurrentPrice
		{
			get
			{
				if (BidsMax == null)
				{
					return MinPrice;
				}
				else return BidsMax.Amount;
			}
		}
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
		public int Mileage;
		public decimal MinPrice;
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
		public string Type;
		public Bid Winner
		{
			get
			{
				if (BidsMax != null && BidsMax.Status == "Winner")
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
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand vCommand = new NpgsqlCommand("SELECT * FROM Lots, Vehicles, VehicleModels, VehicleMakes WHERE Lots.ID = @id AND Lots.Vehicle_ID = Vehicles.ID AND Vehicles.Model_ID = VehicleModels.ID AND VehicleModels.Make_ID = VehicleMakes.ID", connection);
			vCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = id;
			NpgsqlDataReader vReader = vCommand.ExecuteReader();
			vReader.Read();
			AuctionID = (int)vReader[1];
			Color = (string)vReader[8];
			DetailLink = (string)vReader[11];
			ID = (int)vReader[0];
			Make = (string)vReader[17];
			Mileage = (int)vReader[10];
			MinPrice = (decimal)vReader[3];
			Model = (string)vReader[14];
			Trim = (string)vReader[9];
			Type = (string)vReader[15];
			VehicleID = (int)vReader[2];
			VIN = (string)vReader[5];
			Year = (int)vReader[7];
			vReader.Close();

			NpgsqlCommand bCommand = new NpgsqlCommand("SELECT ID FROM Bids WHERE Lot_ID = @id ORDER BY BidTime DESC", connection);
			bCommand.Parameters.Add("@id", NpgsqlTypes.NpgsqlDbType.Integer).Value = ID;
			NpgsqlDataReader bReader = bCommand.ExecuteReader();
			while (bReader.Read())
			{
				Bids.Add(new Bid((int)bReader[0]));
			}
			bReader.Close();

			connection.Close();
		}
	}
}
