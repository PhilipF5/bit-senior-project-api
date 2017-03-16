using System;
using System.Data;
using System.Data.SqlClient;
namespace api.Models
{
	public class Bid
	{
		public decimal Amount;
		public DateTime BidTime;
		public int BuyerID;
		public int ID;
		public int LotID;
		public string Status; // "Placed" "Winner" "Outbid" "Low" "Late" or "Duplicate"

		public Bid(int id)
		{
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				var bidsCommand = new SqlCommand("SELECT * FROM Bids WHERE ID = @id", connection);
				bidsCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
				connection.Open();
				var bidsReader = bidsCommand.ExecuteReader();
				bidsReader.Read();
				ID = (int)bidsReader[0];
				LotID = (int)bidsReader[1];
				Amount = (decimal)bidsReader[3];
				BidTime = DateTime.Parse(bidsReader[4].ToString());
				Status = bidsReader[5].ToString();

				var pCommand = new SqlCommand("SELECT Buyer_ID FROM Participants WHERE ID = @id", connection);
				pCommand.Parameters.Add("@id", SqlDbType.Int).Value = (int)bidsReader[2];
				var pReader = pCommand.ExecuteReader();
				pReader.Read();
				BuyerID = (int)pReader[0];
			}
		}
	}
}
