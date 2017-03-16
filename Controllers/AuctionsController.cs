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
	[Route("api/[controller]")]
	public class AuctionsController : Controller
	{
		[HttpGet("{key}/{id}")]
		public Auction GetAuction(string key, int id)
		{
			if (APIKey.IsValid(key))
			{
				return null;
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
