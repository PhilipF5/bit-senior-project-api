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
	}
}
