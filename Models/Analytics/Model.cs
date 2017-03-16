using System;
namespace api.Models.Analytics
{
	public class Model
	{
		public int AvgPrice;
		public Lot[] Lots;
		public string Make;
		public string Name;
		public string TotalRevenue;
		public int VehiclesSold;

		public Model(int id)
		{
		}
	}
}
