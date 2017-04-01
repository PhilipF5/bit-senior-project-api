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
	public class InputAccount
	{
		public string DealerName;
		public string ContactEmail;
		public string ContactPhone;
		public double CreditAmount;
		public string StreetAddress;
		public string City;
		public string State;
		public string PostalCode;
	}

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

		[HttpPost("{key}/create")]
		public dynamic CreateAccount(string key, [FromBody] InputAccount input)
		{
			if (APIKey.IsManager(key))
			{
				return Account.Create(input);
			}
			else return "Invalid key";
		}
	}
}
