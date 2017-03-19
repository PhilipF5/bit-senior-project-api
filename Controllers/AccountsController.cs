using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Services;
using api.Models;

namespace api.Controllers
{
	[Route("api/accounts")]
	public class AccountsController : Controller
	{
		[HttpGet("{key}")]
		public dynamic GetAccounts(string key)
		{
			if (APIKey.IsManager(key))
			{
				return Account.GetAll();
			}
			else if (APIKey.IsBuyer(key))
			{
				return Account.GetFromKey(key);
			}
			else return "Invalid key";
		}
	}
}
