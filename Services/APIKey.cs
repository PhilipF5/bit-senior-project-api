using System;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace api.Services
{
	public static class APIKey
	{
		public static string Generate()
		{
			var rngCsp = RandomNumberGenerator.Create();
			byte[] key = new byte[8];
			rngCsp.GetBytes(key);
			string hex = BitConverter.ToString(key).ToLower();
			return hex.Replace("-", "");
		}

		public static bool IsBuyer(string key)
		{
			
		}

		public static bool IsManager(string key)
		{
			
		}

		public static bool IsValid(string key)
		{

		}
	}
}
