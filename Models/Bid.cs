using System;
namespace api.Models
{
	public class Bid
	{
		public int Amount;
		public DateTime BidTime;
		public int BuyerID;
		public int ID;
		public int LotID;
		public string Status; // "Placed" "Winner" "Outbid" "Low" "Late" or "Duplicate"

		public Bid(int id)
		{
		}
	}
}
