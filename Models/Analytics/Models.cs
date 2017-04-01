using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace api.Models.Analytics
{
	public class Models
	{
		private List<Auction> Auctions;
		public List<string> ModelNames = new List<string>();
		public List<decimal> SalesByRevenue = new List<decimal>();
		public List<int> SalesByVolume = new List<int>();

		public Models()
		{
			using (var connection = new NpgsqlConnection(Environment.GetEnvironmentVariable("CUSTOMCONNSTR_bit4454postgres")))
			{
				var aCommand = new NpgsqlCommand("SELECT MakeName, ModelName FROM VehicleMakes, VehicleModels WHERE VehicleMakes.ID = VehicleModels.Make_ID", connection);
				connection.Open();
				var aReader = aCommand.ExecuteReader();
				while (aReader.Read())
				{
					ModelNames.Add(aReader[0].ToString() + " " + aReader[1].ToString());
				}
				aReader.Close();
			}

			Auctions = Auction.GetAll();
			foreach (string model in ModelNames)
			{
				decimal revenue = 0;
				int volume = 0;
				foreach (Auction auction in Auctions)
				{
					foreach (Lot lot in auction.Lots)
					{
						if (lot.Status == "Sold" && (lot.Make + " " + lot.Model) == model)
						{
							revenue += lot.Winner.Amount;
							volume++;
						}
					}
				}
				SalesByRevenue.Add(revenue);
				SalesByVolume.Add(volume);
			}
		}
	}
}
