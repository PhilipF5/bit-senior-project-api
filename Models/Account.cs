using System;
namespace api.Models
{
	public class Account
	{
		public string Address;
		public int AvailableCredit;
		public Buyer[] Buyers; // Not serialized for mobile users
		public string City;
		public string ContactEmail;
		public string ContactPhone;
		public int ID;
		public string Owner;
		public string PostalCode;
		public string State;
		public string StateCode;
		public int TotalCredit;
		public int TotalSpent;
		public int UsedCredit;

		public Account(int id)
		{
		}
	}
}
