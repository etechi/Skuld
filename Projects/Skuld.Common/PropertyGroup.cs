using System;
using System.Collections.Generic;
using System.Linq;
using SF;
namespace Skuld
{
	public class PropertyGroup
	{
		public string Name { get; set; }
		public Dictionary<string, string>[] Rows { get; set; }
		public DateTime? Time { get; set; }
		public DateTime? NextUpdateTime { get; set; }
		public Symbol Symbol { get; set; }
		public override string ToString()
		{
			return $"{Symbol} {Name} {Time:yyyy-MM-dd} {Rows.Select(r=>r.Select(p => p.Key + "=" + p.Value).Join(";")).Join("&")}";
		}
	}
}
