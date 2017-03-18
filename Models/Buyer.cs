using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
namespace api.Models
{
	public class Buyer
	{
		public int AccountID;
		public int AuctionCount
		{
			get
			{
				return ParticipantID.Count;
			}
		}
		public List<Bid> Bids = new List<Bid>();
		public int BidsCount
		{
			get
			{
				return Bids.Count;
			}
		}
		public decimal BidsMax = 0;
		public decimal BidsMin = 100000;
		public int ID; // Buyers table
		public string FirstName;
		public string FullName
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}
		public string LastName;
		private List<int> ParticipantID = new List<int>();
		public decimal TotalSpent = 0;
		public string Username;

		public Buyer(int id)
		{
			using (var connection = new SqlConnection("Server=tcp:auctionit.database.windows.net,1433;Initial Catalog=auctionitdb;Persist Security Info=False;User ID=bit4454;Password=4Fy>oj@8&8;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
			{
				// Load Buyers table data
				var buyersCommand = new SqlCommand("SELECT * FROM Buyers WHERE ID = @id", connection);
				buyersCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;
				connection.Open();
				var buyersReader = buyersCommand.ExecuteReader();
				buyersReader.Read();
				ID = (int)buyersReader[0];
				FirstName = (string)buyersReader[1];
				LastName = (string)buyersReader[2];
				AccountID = (int)buyersReader[3];

				// Load Auctions table data
				var auctionsCommand = new SqlCommand("SELECT ID FROM Participants WHERE Buyer_ID = @id", connection);
				auctionsCommand.Parameters.Add("@id", SqlDbType.Int).Value = ID;
				var auctionsReader = auctionsCommand.ExecuteReader();
				while (auctionsReader.Read())
				{
					ParticipantID.Add((int)auctionsReader[0]);
				}

				// Load Users table data
				var usersCommand = new SqlCommand("SELECT Username FROM Users WHERE Buyer_ID = @id", connection);
				usersCommand.Parameters.Add("@id", SqlDbType.Int).Value = ID;
				var usersReader = usersCommand.ExecuteReader();
				usersReader.Read();
				Username = (string)usersReader[0];

				// Load Bids table data
				var bidsCommand = new SqlCommand("SELECT * FROM Bids WHERE Participant_ID IN ({Range})", connection);
				bidsCommand.AddArrayParameters(ParticipantID.ToArray(), "Range");
				//Console.WriteLine(bidsCommand.CommandText); // debugging
				var bidsReader = bidsCommand.ExecuteReader();
				while (bidsReader.Read())
				{
					Bids.Add(new Bid((int)bidsReader[0]));
				}
				if (BidsCount > 0)
				{
					foreach (Bid bid in Bids)
					{
						if (bid.Amount > BidsMax)
						{
							BidsMax = bid.Amount;
						}
						if (bid.Amount < BidsMin)
						{
							BidsMin = bid.Amount;
						}
						if (bid.Status == "Winner")
						{
							TotalSpent += bid.Amount;
						}
					}
				}
				else
				{
					BidsMin = 0;
				}
			}
		}
	}
}
