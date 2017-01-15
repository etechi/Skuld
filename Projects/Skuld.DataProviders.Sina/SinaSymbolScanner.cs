using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using SF;
namespace Skuld.DataProvider.Sina
{
	public class SinaSymbolScanner
	{
		class record
		{
			public string symbol { get; set; }
			public string code { set; get; }
			public string name { get; set; }
		}
		async Task<record[]> Get(int page, string node)
		{
			var args = new Dictionary<string, string> {
				{ "PAGE", page.ToString() },
				{ "COUNT", "80" },
				{"NODE",node }
				};
			var url = new Uri(SimpleTemplate.Eval(Setting.SymbolScanUrl, args));
			return await url.Get<record[]>(UriExtension.GBK);
		}
		static string[] nodes = new[]
		{
			"hs_a",
			"hs_s"
            //"sh_a",
            //"sh_b",
            //"sz_a",
            //"sz_b",
            //"sz_a",
            //"sz_a",
        };
		public string Name { get { return "sina"; } }
		IObservable<record> GetAllRecords(string node)
		{
			var url = Setting.SymbolScanUrl;
			var pages = Enumerable.Range(1, 100)
				.ToObservable()
				.Delay(i => Observable.Timer(TimeSpan.FromMilliseconds(i * 2*100)))
				;
			var result = from page in pages
						 from rs in Get(page, node).ToObservable()
						 select rs;
			return from rs in result.TakeWhile(rs => rs != null && rs.Length > 0)
				   from r in rs
				   select r;
		}
		public IObservable<Symbol> Scan()
		{
			var sha = new SymbolScope { ScopeCode = "sh", Type = SymbolType.Stock };
			var shi = new SymbolScope { ScopeCode = "sh", Type = SymbolType.Index };
			var sza = new SymbolScope { ScopeCode = "sz", Type = SymbolType.Stock };
			var szi = new SymbolScope { ScopeCode = "sz", Type = SymbolType.Index };
			var a = GetAllRecords("hs_a").Select(
					r => new Symbol
					{
						Scope = r.symbol.StartsWith("sh") ? sha : sza,
						Code = r.code,
						Name = r.name
					});
			var i = GetAllRecords("hs_s").Select(
					r => new Symbol
					{
						Scope = r.symbol.StartsWith("sh") ? shi : szi,
						Code = r.code,
						Name = r.name
					});
			return a.Concat(i);
		}
		public SinaSetting Setting { get; }
		public SinaSymbolScanner(SinaSetting Setting)
		{
			this.Setting = Setting;
		}
	}
}
