using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public static class Json
	{
		public static T Parse<T>(string str)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
		}
		public static string Stringify<T>(T Object)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(Object);
		}
	}
}
