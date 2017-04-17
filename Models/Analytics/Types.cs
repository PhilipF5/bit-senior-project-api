﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using api.Services;

namespace api.Models.Analytics
{
	public class Types
	{
		private List<Auction> Auctions;
		public List<decimal> SalesByRevenue = new List<decimal>();
		public List<int> SalesByVolume = new List<int>();
		public List<string> TypeNames = new List<string>();

		public Types()
		{
			NpgsqlConnection connection = new Database().Connection;

			NpgsqlCommand aCommand = new NpgsqlCommand("SELECT DISTINCT category FROM VehicleModels", connection);
			NpgsqlDataReader aReader = aCommand.ExecuteReader();
			while (aReader.Read())
			{
				TypeNames.Add(aReader[0].ToString());
			}
			aReader.Close();

			Auctions = Auction.GetAll();
			foreach (string type in TypeNames)
			{
				decimal revenue = 0;
				int volume = 0;
				foreach (Auction auction in Auctions)
				{
					foreach (Lot lot in auction.Lots)
					{
						if (lot.Status == "Sold" && lot.Type == type)
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
