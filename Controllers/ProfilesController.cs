using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using api.Models;
using api.Services;

namespace api.Controllers
{
	public class InputBuyer
	{
		public string FirstName;
		public string LastName;
		public string Username;
		public int AccountID;
	}

	[Route("api/profiles")]
	public class ProfilesController : Controller
	{
		[HttpGet("{key}/{id:int}")]
		public dynamic GetByID(string key, int id)
		{
			if (APIKey.IsManager(key))
			{
				return new Buyer(id);
			}
			else return false;
		}

		[HttpGet("{key}")]
		public dynamic GetByKey(string key)
		{
			var id = APIKey.GetBuyerID(key);
			if (id != 0)
			{
				return new Buyer(id);
			}
			else return false;
		}

		[HttpPost("{key}/create")]
		public dynamic Create(string key, [FromBody] InputBuyer input)
		{
			if (APIKey.IsManager(key))
			{
				return Buyer.Create(input);
			}
			else return "Invalid key";
		}
	}
}
