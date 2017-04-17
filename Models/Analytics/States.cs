using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using api.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace api.Models.Analytics
{
	public class States
	{
		private List<Auction> Auctions;
		public List<decimal> SalesByRevenue = new List<decimal>();
		public List<int> SalesByVolume = new List<int>();
		public List<string> StateCodes = new List<string>();
		public List<string> StateNames = new List<string>();

		public States()
		{
			Auctions = Auction.GetAll();
			foreach (Tuple<string, string> state in StateInfo.StateData)
			{
				decimal revenue = 0;
				int volume = 0;
				StateNames.Add(state.Item1);
				StateCodes.Add(state.Item2);
				foreach (Auction auction in Auctions)
				{
					if (auction.StateCode == state.Item2)
					{
						foreach (Lot lot in auction.Lots)
						{
							if (lot.Status == "Sold")
							{
								revenue += lot.Winner.Amount;
								volume++;
							}
						}
					}
				}
				SalesByRevenue.Add(revenue);
				SalesByVolume.Add(volume);
			}
		}
	}
}
