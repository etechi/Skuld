﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using SF;
using SF.Core;
using SF.Sys;
using SF.Sys.HttpClients;

namespace Skuld.DataProviders.Sina
{
	public class SinaKLineFrameDigger : DataProviders.IKLineFrameDigger
	{
		public SinaSetting Setting { get; }

		public IEnumerable<SymbolScope> Supports
		{
			get
			{
				return new[]
				{
					new SymbolScope
					{
						ScopeCode="sh",
						Type=SymbolType.Index
					},
					new SymbolScope
					{
						ScopeCode="sh",
						Type=SymbolType.Stock
					},
					new SymbolScope
					{
						ScopeCode="sz",
						Type=SymbolType.Index
					},
					new SymbolScope
					{
						ScopeCode="sz",
						Type=SymbolType.Stock
					},
					new SymbolScope
					{
						ScopeCode="hs",
						Type=SymbolType.Index
					}
				};
			}
		}
		IHttpClient HttpClient { get; }
		public SinaKLineFrameDigger(SinaSetting Setting, IHttpClient HttpClient)
		{
			this.HttpClient = HttpClient;
			this.Setting = Setting;
		}
		class record
		{
			public string day { get; set; }
			public string open { get; set; }
			public string high { get; set; }
			public string low { get; set; }
			public string close { get; set; }
			public string volume { get; set; }
		}
		class adjresult
		{
			public int total { get; set; }
			public Dictionary<string, string> data { get; set; }

		}
		async Task<KLineFrame[]> GetStockAndIndexKLineFrame(string symbol, int scale, int count, bool adjuest)
		{
			//public string AdjuestPriceUrl { get; private set; } = "http://finance.sina.com.cn/realstock/newcompany/{SYMBOL}/phfq.js";
			//public string TradePriceUrl { get; private set; } = "http://money.finance.sina.com.cn/quotes_service/api/jsonp_v2.php/a/CN_MarketData.getKLineData?symbol={SYMBOL}&scale={SCALE}&ma=no&datalen={COUNT}";
			var args = new Dictionary<string, object> {
				{ "SCALE", scale.ToString() },
				{ "COUNT", count.ToString() },
				{ "SYMBOL", symbol }
				};

			var url = new Uri(Setting.StockAndIndexTradePriceUrl.Replace(args));
			//var str = await url.GetString(Encoding.ASCII);
			var str = await HttpClient.From(url).GetString();
			if (string.IsNullOrEmpty(str))
				return Array.Empty<KLineFrame>();

			var ii = str.IndexOf('[');
			if (ii == -1)
				return Array.Empty<KLineFrame>();
			var ei = str.LastIndexOf(']');
			if(ei==-1)
				return Array.Empty<KLineFrame>();

			var frames = Json.Parse<record[]>(str.Substring(ii, ei - ii + 1)).Select(r =>
				new KLineFrame
				{
					Close = float.Parse(r.close),
					High = float.Parse(r.high),
					Low = float.Parse(r.low),
					Open = float.Parse(r.open),
					Volume = float.Parse(r.volume),
					AdjuestRate = 1,
					Time = DateTime.ParseExact(r.day, r.day.Length == 10 ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.AssumeLocal)
				}
			);

			var re = frames.ToArray();
			if (adjuest)
			{
				var adjurl = new Uri(Setting.StockAndIndexAdjuestPriceUrl.Replace(args));
				//var adjstr = await adjurl.GetString(Encoding.ASCII);
				var adjstr = await HttpClient.From(adjurl).GetString();
				if (adjstr == null)
					return null;

				var i = adjstr.IndexOf('{');
				var e = adjstr.LastIndexOf('}');
				var adj = Json.Parse<adjresult>(adjstr.Substring(i, e - i + 1));
				var dic = adj.data;


				var last_adj_rate = 1f;
				foreach (var f in re)
				{
					string spr;
					var key = f.Time.ToString("_yyyy_MM_dd");
					if (dic.TryGetValue(key, out spr))
					{
						var pr = float.Parse(spr);
						last_adj_rate = f.AdjuestRate = pr / f.Close;
					}
					else
						f.AdjuestRate = last_adj_rate;
				}
			}
			return re;

		}
		class FundRecord
		{
			public string fbrq { get; set; }
			public string jjjz { get; set; }
			public string ljjz { get; set; }
		}


		async Task<KLineFrame[]> GetFundKLineFrame(string symbol, int scale, int count)
		{
			//public string AdjuestPriceUrl { get; private set; } = "http://finance.sina.com.cn/realstock/newcompany/{SYMBOL}/phfq.js";
			//public string FundPriceUrl { get; set; } = "http://stock.finance.sina.com.cn/fundInfo/api/openapi.php/CaihuiFundInfoService.getNav?callback=abc&symbol=519690&datefrom=&dateto=&page={PAGE}&num={COUNT}";
			var args = new Dictionary<string, object> {
				{ "PAGE", "1" },
				{ "COUNT", count.ToString() },
				{ "SYMBOL", symbol }
				};

			var url = new Uri(Setting.FundPriceUrl.Replace(args));
			//var str = await url.GetString(Encoding.ASCII);
			var str = await HttpClient.From(url).GetString();
			if (string.IsNullOrEmpty(str))
				return Array.Empty<KLineFrame>();
			str = str.Substring("\"data\":[{", -2, "}]", 2);
			if (str.IsNullOrEmpty())
				return Array.Empty<KLineFrame>();

			var frames = Json.Parse<FundRecord[]>(str).Select(r =>
			{
				var price = r.jjjz.ToFloat();
				var AdjuestRate = r.ljjz==null?1:r.ljjz.ToFloat() / price;
				var time = r.fbrq.ToDateTime(
					r.fbrq.Length == 10 ? "yyyy-MM-dd" : "yyyy-MM-dd HH:mm:ss",
					System.Globalization.DateTimeStyles.AssumeLocal
					);

				return new KLineFrame
				{
					Close = price,
					High = price,
					Low = price,
					Open = price,
					Volume = 0,
					AdjuestRate = AdjuestRate,
					Time = time
				};
			}
			);

			var re = frames.ToArray();
			return re;

		}
		public IObservable<KLineFrame> Dig(
			Symbol Symbol,
			int TimeInterval,
			DateTime BeginTime,
			DateTime EndTime
		   )
		{
			int scale;
			int count;
			var curTime = DateTime.Now;
			if (TimeInterval == TimeIntervals.Day)
			{
				scale = 240;
				count = (int)curTime.Subtract(BeginTime).TotalDays + 1;
			}
			else if (TimeInterval == TimeIntervals.M5)
			{
				scale = 5;
				count = 12 * 4 * ((int)curTime.Subtract(BeginTime).TotalDays + 1);
			}
			else
				throw new NotSupportedException();
			Task<KLineFrame[]> frames = null;
			if (Symbol.Scope.ScopeCode == "sh" || Symbol.Scope.ScopeCode == "sz")
			{
				var symbol = Symbol.Scope.ScopeCode + Symbol.Code;
				frames = GetStockAndIndexKLineFrame(symbol, scale, count, Symbol.Scope.Type == SymbolType.Stock);
			}
			else if (Symbol.Scope.Type == SymbolType.Fund)
			{
				frames = GetFundKLineFrame(Symbol.Code, scale, count);
			}

			if (frames == null)
				return Observable.Empty<KLineFrame>();

			return from rs in frames.ToObservable()
				   from r in rs
				   where r.Time >= BeginTime && r.Time <= EndTime
				   select r;
		}

	}
}
