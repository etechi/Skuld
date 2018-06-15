using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using SF.Sys;
using SF.Sys.HttpClients;

namespace Skuld.DataProviders.Sina
{
	public class SinaSymbolScanner : DataProviders.ISymbolScanner
	{
		class StockAndIndexRecord
		{
			public string symbol { get; set; }
			public string code { set; get; }
			public string name { get; set; }
		}
		async Task<StockAndIndexRecord[]> GetStockAndIndex(int page, string node)
		{
			var args = new Dictionary<string, object> {
				{ "PAGE", page.ToString() },
				{ "COUNT", "80" },
				{"NODE",node }
				};
			var url = new Uri(Setting.StockAndIndexScanUrl.Replace(args));
			//System.Text.Encoding.GetEncoding

			return Json.Parse<StockAndIndexRecord[]>(await HttpClient.From(url).Execute(async resp=>
			{
				var re=await resp.Content.ReadAsStringAsync();
				return re;
			}));
		}
		IObservable<StockAndIndexRecord> GetAllStockAndIndexRecords(string node)
		{
			//return Observable.Empty<StockAndIndexRecord>();

			var pages = Enumerable.Range(1, 100)
				.ToObservable()
				.Delay(i => Observable.Timer(TimeSpan.FromMilliseconds(i * 2 * 100)))
				;
			var result = from page in pages
						 from rs in GetStockAndIndex(page, node).ToObservable()
						 select rs;
			return from rs in result.TakeWhile(rs => rs != null && rs.Length > 0)
				   from r in rs
				   select r;
		}



		class FundRecord
		{
			public string symbol { get; set; }
			public string name { get; set; }
		}
		async Task<FundRecord[]> GetFund(int page)
		{
			var args = new Dictionary<string, object> {
				{ "PAGE", page.ToString() },
				{ "COUNT", "200" }
				};
			var url = new Uri(Setting.FundSymbolScanUrl.Replace(args));
			var str = await HttpClient.From(url).Execute(async resp =>
			{
				var buf = await resp.Content.ReadAsByteArrayAsync();
				return System.Text.Encoding.GetEncoding("GBK").GetString(buf);
			});
			return Json.Parse<FundRecord[]>(str.Substring("data:[",-1, "}]",2));
		}
		IObservable<FundRecord> GetAllFundRecords()
		{
			var pages = Enumerable.Range(1, 100)
				.ToObservable()
				.Delay(i => Observable.Timer(TimeSpan.FromMilliseconds(i * 2 * 100)))
				;
			var result = from page in pages
						 from rs in GetFund(page).ToObservable()
						 select rs;
			return from rs in result.TakeWhile(rs => rs != null && rs.Length > 0)
				   from r in rs
				   select r;
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
		
		public IObservable<Symbol> Scan()
		{
			var sha = new SymbolScope { ScopeCode = "sh", Type = SymbolType.Stock };
			var shi = new SymbolScope { ScopeCode = "sh", Type = SymbolType.Index };
			var sza = new SymbolScope { ScopeCode = "sz", Type = SymbolType.Stock };
			var szi = new SymbolScope { ScopeCode = "sz", Type = SymbolType.Index };
			var fund = new SymbolScope { ScopeCode = "cn", Type = SymbolType.Fund };
			var a = GetAllStockAndIndexRecords("hs_a").Select(
					r => new Symbol
					{
						Scope = r.symbol.StartsWith("sh") ? sha : sza,
						Code = r.code,
						Name = r.name
					});
			var i = GetAllStockAndIndexRecords("hs_s").Select(
					r => new Symbol
					{
						Scope = r.symbol.StartsWith("sh") ? shi : szi,
						Code = r.code,
						Name = r.name
					});
			var f = GetAllFundRecords().Select(
				r => new Symbol
				{
					Scope= fund,
					Code=r.symbol,
					Name=r.name
				});
			return a.Concat(i).Concat(f);
		}
		public SinaSetting Setting { get; }
		public IHttpClient HttpClient { get; }
		public SinaSymbolScanner(SinaSetting Setting, IHttpClient HttpClient)
		{
			this.HttpClient = HttpClient;
			this.Setting = Setting;
		}
	}
}
