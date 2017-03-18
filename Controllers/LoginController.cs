using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace api.Controllers
{
	[Route("api/login")]
	public class LoginController : Controller
	{
		[HttpPost("mobile")]
		public Login Mobile([FromBody] string creds)
		{
			string[] credsArray = creds.Split(' ');
			return new Login(credsArray[0], credsArray[1]);
		}

		[HttpPost("web")]
		public Login Web([FromBody] string creds)
		{
			string[] credsArray = creds.Split(' ');
			return new Login(credsArray[0], credsArray[1], "admin");
		}
	}
}
