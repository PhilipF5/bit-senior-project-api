using System;
namespace api.Models
{
	public class State
	{
		public int AvgPrice;
		public Auction[] Auctions;
		public string Code;
		public string Name;
		public int Participants;
		public string TotalRevenue;
		public int VehiclesSold;

		public State(int id)
		{
		}
	}
}
