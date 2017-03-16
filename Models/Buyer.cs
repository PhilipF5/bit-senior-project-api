using System;
namespace api.Models
{
	public class Buyer
	{
		public Account Account;
		public int AuctionCount;
		public Bid[] Bids;
		public int BidsCount;
		public int BidsMax;
		public int BidsMin;
		public int ID; // Buyers table
		public string FirstName;
		public string FullName;
		public string LastName;
		public int TotalSpent;
		public string Username;

		public Buyer(int id)
		{
		}
	}
}
