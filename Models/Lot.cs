using System;
namespace api.Models
{
	public class Lot
	{
		public int AuctionID;
		public Bid[] Bids;
		public Bid BidsMax;
		public string Color;
		public string Desc; // Year + Make + Model + Trim
		public string DetailLink;
		public int ID;
		public string Make;
		public int MinPrice;
		public string Model;
		public string Status;
		public string Trim;
		public Bid Winner;
		public int VehicleID;
		public string VIN;
		public int Year;

		public Lot(int id)
		{
		}
	}
}
