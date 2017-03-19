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
		[HttpGet("bid/{key}/{lotID}/{amount}")]
		public dynamic Bid(string key, int lotID, decimal amount)
		{
			if (!APIKey.IsBuyer(key))
			{
				return "Invalid key";
			}
			return Models.Bid.Place(key, lotID, amount);
		}

		[HttpGet("{key}/{id}")]
		public dynamic GetAuction(string key, int id)
		{
			if (!APIKey.IsValid(key))
			{
				return "Invalid key";
			}
			Auction auction = new Auction(id);
			if (APIKey.IsManager(key))
			{
				auction.SerializeBuyers = true;
			}
			return auction;
		}
	}
}
