using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Services;
using api.Models.Analytics;

namespace api.Controllers
{
	[Route("api/auctions")]
	public class AuctionsController : Controller
	{
		[HttpGet("{key}/{id}/accept")]
		public dynamic AcceptBid(string key, int id)
		{
			if (!APIKey.IsManager(key))
			{
				return "Invalid key";
			}
			return Models.Bid.Accept(id);
		}

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

		[HttpGet("{key}")]
		public dynamic GetAllAuctions(string key)
		{
			if (!APIKey.IsValid(key))
			{
				return "Invalid key";
			}
			else return Auction.GetAll(APIKey.IsManager(key));
		}

		[HttpGet("{key}/states")]
		public dynamic GetStateTotals(string key)
		{
			if (!APIKey.IsManager(key))
			{
				return "Invalid key";
			}
			else return new States();
		}

		[HttpGet("{key}/models")]
		public dynamic GetModelTotals(string key)
		{
			if (!APIKey.IsManager(key))
			{
				return "Invalid key";
			}
			else return new Models.Analytics.Models();
		}

		[HttpGet("{key}/types")]
		public dynamic GetTypeTotals(string key)
		{
			if (!APIKey.IsManager(key))
			{
				return "Invalid key";
			}
			else return new Types();
		}
	}
}
