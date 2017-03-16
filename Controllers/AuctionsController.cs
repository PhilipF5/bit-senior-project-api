using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;

namespace api.Controllers
{
	[Route("api/auctions")]
	public class AuctionsController : Controller
	{
		[HttpGet("{key}/{id}")]
		public dynamic GetAuction(string key, int id)
		{
			if (!APIKey.IsValid(key))
			{
				return "Invalid key";
			}
			Auction auction = new Auction(id);
			return auction.ID;
			if (APIKey.IsManager(key))
			{
				auction.SerializeBuyers = true;
			}
			return auction;
		}
	}
}
