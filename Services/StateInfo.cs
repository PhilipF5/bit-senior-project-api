using System;
using System.Runtime.InteropServices;
namespace api.Services
{
	public static class StateInfo
	{
		public static Tuple<string, string>[] StateData = {
			Tuple.Create("Alabama", "AL"),
			Tuple.Create("Alaska", "AK"),
			Tuple.Create("Arizona", "AZ"),
			Tuple.Create("Arkansas", "AR"),
			Tuple.Create("California", "CA"),
			Tuple.Create("Colorado", "CO"),
			Tuple.Create("Connecticut", "CT"),
			Tuple.Create("Delaware", "DE"),
			Tuple.Create("Florida", "FL"),
			Tuple.Create("Georgia", "GA"),
			Tuple.Create("Hawaii", "HI"),
			Tuple.Create("Idaho", "ID"),
			Tuple.Create("Illinois", "IL"),
			Tuple.Create("Indiana", "IN"),
			Tuple.Create("Iowa", "IA"),
			Tuple.Create("Kansas", "KS"),
			Tuple.Create("Kentucky", "KY"),
			Tuple.Create("Louisiana", "LA"),
			Tuple.Create("Maine", "ME"),
			Tuple.Create("Maryland", "MD"),
			Tuple.Create("Massachusetts", "MA"),
			Tuple.Create("Michigan", "MI"),
			Tuple.Create("Minnesota", "MN"),
			Tuple.Create("Mississippi", "MS"),
			Tuple.Create("Missouri", "MO"),
			Tuple.Create("Montana", "MT"),
			Tuple.Create("Nebraska", "NE"),
			Tuple.Create("Nevada", "NV"),
			Tuple.Create("New Hampshire", "NH"),
			Tuple.Create("New Jersey", "NJ"),
			Tuple.Create("New Mexico", "NM"),
			Tuple.Create("New York", "NY"),
			Tuple.Create("North Carolina", "NC"),
			Tuple.Create("North Dakota", "ND"),
			Tuple.Create("Ohio", "OH"),
			Tuple.Create("Oklahoma", "OK"),
			Tuple.Create("Oregon", "OR"),
			Tuple.Create("Pennsylvania", "PA"),
			Tuple.Create("Rhode Island", "RI"),
			Tuple.Create("South Carolina", "SC"),
			Tuple.Create("South Dakota", "SD"),
			Tuple.Create("Tennessee", "TN"),
			Tuple.Create("Texas", "TX"),
			Tuple.Create("Utah", "UT"),
			Tuple.Create("Vermont", "VT"),
			Tuple.Create("Virginia", "VA"),
			Tuple.Create("Washington", "WA"),
			Tuple.Create("West Virginia", "WV"),
			Tuple.Create("Wisconsin", "WI"),
			Tuple.Create("Wyoming", "WY"),
			Tuple.Create("D.C.", "DC")
		};

		public static string GetCode(string name)
		{
			foreach (Tuple<string, string> state in StateData)
			{
				if (state.Item1 == name)
				{
					return state.Item2;
				}
			}
			return null;
		}

		public static string GetName(string code)
		{
			foreach (Tuple<string, string> state in StateData)
			{
				if (state.Item2 == code)
				{
					return state.Item1;
				}
			}
			return null;
		}
	}
}
