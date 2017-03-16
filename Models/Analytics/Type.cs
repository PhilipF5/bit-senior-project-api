using System;
namespace api.Models.Analytics
{
	public class Type
	{
		public int AvgPrice;
		public Lot[] Lots;
		public string Name;
		public string TotalRevenue;
		public int VehiclesSold;

		public Type(int id)
		{
		}
	}
}
