using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using SF;
using System.Text.RegularExpressions;
using System.Text;
using SF.Sys.Linq;
using SF.Sys;
using SF.Sys.HttpClients;

namespace Skuld.DataProviders.Sina
{
	
	public class SinaSymbolPropertyDigger : DataProviders.ISymbolPropertyDigger
	{
		static Regex RegHtmlClear = new Regex("<[^>]+>");
		async Task<string> Dig(Symbol symbol,string type,string startContent)
		{
			//Console.WriteLine($"start dig {symbol} {type}");
			var args = new Dictionary<string, object> {
				{"TYPE",type },
				{"SYMBOL",symbol.Scope.ScopeCode+symbol.Code }
				};
			var url = new Uri(Setting.PropertyUrl.Replace(args));
			var html = await HttpClient.From(url).GetString();
			var start = html.IndexOf(startContent);
			if (start == -1)
				return null;
			start += startContent.Length;
			var end = html.IndexOf("实时行情", start);
			if (end == -1)
				return null;
			var content = html.Substring(start, end - start)
				.Replace("<br/>", "\n")
				.Replace("&nbsp;", " ");
			content = RegHtmlClear.Replace(content, "");
			//Console.WriteLine($"end dig {symbol} {type}");
			return content;
		}
		enum ParseType
		{
			Normal,
			MultiGroups,
			Table
		}
		IEnumerable<PropertyGroup> ParsePropertyGroups(Symbol symbol,string content, string group, ParseType Type, string DateProperty,string[] RemoveProps=null)
		{
			if (Type==ParseType.MultiGroups)
			{
				content = content.Split('\n').Select(l => l.Trim()).Join("\n");
				foreach (var grp in content.Replace("\n\n", "\0").Split('\0'))
					foreach (var vg in ParsePropertyGroups(symbol,grp, group, ParseType.Normal, DateProperty))
						yield return vg;
				yield break;
			}
			else if (Type == ParseType.Table)
			{
				if (DateProperty != null)
				{
					if (RemoveProps == null)
						RemoveProps = new[] { DateProperty };
					else
						RemoveProps = RemoveProps.Concat(new[] { DateProperty }).ToArray();
				}
				var grp = new PropertyGroup
				{
					Name = group,
					Symbol = symbol,
					Rows =
						content.Split('\n').Select(l => l.Trim()).Join("\n").Replace("\n\n", "\0").Split('\0').Select(
							ctn => ParsePropertyGroups(symbol, ctn, group, ParseType.Normal, null, RemoveProps).FirstOrDefault()?.Rows?.FirstOrDefault()
							).Where(r=>r!=null).ToArray()
				};
				if (DateProperty != null)
				{
					DateTime time;
					var sdate = content.SplitAndNormalizae('\n').Select(l => l.Split2(':')).Where(p => p.Item1.Trim() == DateProperty).Select(p => p.Item2).FirstOrDefault();
					if(sdate!=null && DateTime.TryParse(sdate, out time))
						grp.Time = time;
				}
				if(grp.Rows.Length>0)
					yield return grp;
				yield break;
			}

			var props =
				(from line in content.SplitAndNormalizae('\n')
				 let pair = line.Split2(':')
				 where pair.Item2!=null
				 let key = pair.Item1.Trim()
				 let value = pair.Item2.Trim()
				 where key.Length > 0 && value.Length > 0
				 select new { key = key, value = value }
				 ).ToDictionary(p => p.key, p => p.value);

			if (RemoveProps != null)
				foreach (var rp in RemoveProps)
					props.Remove(rp);

			if (props.Count == 0)
				yield break;

			var g = new PropertyGroup
			{
				Symbol=symbol,
				Rows = new[] { props },
				Name=group
			};
			if (DateProperty != null)
			{
				DateTime time; 
				string stime;
				if(props.TryGetValue(DateProperty, out stime) && DateTime.TryParse(stime,out time))
				{
					props.Remove(DateProperty);
					g.Time = time;
				}
			}
			yield return g;
		}
		async Task<IEnumerable<PropertyGroup>> DigCompany(Symbol symbol)
		{
			var content = await Dig(symbol, "company", "【公司概况】");
			if (content == null)
				return Array.Empty<PropertyGroup>();
			return ParsePropertyGroups(symbol,content, "公司概况", ParseType.Normal, null);
		}
		async Task<IEnumerable<PropertyGroup>> DigCapital(Symbol symbol)
		{
			var content = await Dig(symbol, "capital", "【股本结构】");
			if (content == null)
				return Array.Empty<PropertyGroup>();
			return ParsePropertyGroups(symbol, content, "股本结构", ParseType.Normal, "公告日期");
		}
		async Task<IEnumerable<PropertyGroup>> DigHolder(Symbol symbol)
		{
			var content = await Dig(symbol, "holder", "【十大股东】");
			if (content == null)
				return Array.Empty<PropertyGroup>();
			return ParsePropertyGroups(symbol, content, "十大股东", ParseType.Table, "公告日期");
		}
		async Task<IEnumerable<PropertyGroup>> DigAbstr(Symbol symbol)
		{
			var content = await Dig(symbol, "abstr", "【财务摘要】");
			if (content == null)
				return Array.Empty<PropertyGroup>();
			return ParsePropertyGroups(symbol, content, "财务摘要", ParseType.MultiGroups, "截止日期",new[] { "公告日期" })
				.Select(g =>{
					if (g.Time.HasValue)
						g.NextUpdateTime = g.Time.Value.AddDays(1).AddMonths(3).AddDays(-1);
					return g;
				});
		}
		async Task<IEnumerable<PropertyGroup>> DigFinance(Symbol symbol)
		{
			var content = await Dig(symbol, "finance", "【分红信息】");
			if (content == null)
				return Array.Empty<PropertyGroup>();
			return ParsePropertyGroups(symbol, content, "分红信息", ParseType.MultiGroups, "公告日期");
		}
		async Task<IEnumerable<PropertyGroup>> DigLiab(Symbol symbol)
		{
			var content = await Dig(symbol, "liab", "【资产负债】");
			if (content == null)
				return Array.Empty<PropertyGroup>();
			return ParsePropertyGroups(symbol, content, "资产负债", ParseType.MultiGroups, "报表日期", new[] {"单位"})
				.Select(g => {
					if (g.Time.HasValue)
						g.NextUpdateTime = g.Time.Value.AddDays(1).AddMonths(3).AddDays(-1);
					return g;
				});
		}
		async Task<IEnumerable<PropertyGroup>> DigProfit(Symbol symbol)
		{
			var content = await Dig(symbol, "profit", "【利润简表】");
			if (content == null)
				return Array.Empty<PropertyGroup>();
			return ParsePropertyGroups(symbol, content, "利润简表", ParseType.MultiGroups, "报表日期", new[] { "单位" })
				.Select(g => {
					if (g.Time.HasValue)
						g.NextUpdateTime = g.Time.Value.AddDays(1).AddMonths(3).AddDays(-1);
					return g;
				});
		}
		async Task<IEnumerable<PropertyGroup>> DigCash(Symbol symbol)
		{
			var content = await Dig(symbol, "cash", "【现金流量】");
			if (content == null)
				return Array.Empty<PropertyGroup>();
			return ParsePropertyGroups(symbol, content, "现金流量", ParseType.MultiGroups, "报表日期", new[] { "单位" })
				.Select(g => {
					if (g.Time.HasValue)
						g.NextUpdateTime = g.Time.Value.AddDays(1).AddMonths(3).AddDays(-1);
					return g;
				}); 
		}
		class Digger
		{
			public string Name { get; set; }
			public Func<Symbol, Task<IEnumerable<PropertyGroup>>> Func { get; set; }
		}
		Digger NewDigger(string Name, Func<Symbol, Task<IEnumerable<PropertyGroup>>> Func)
		{
			return new Digger { Name = Name, Func = Func };
		}
		static bool SkipDigger(Dictionary<string, DateTime> UpdateTime,string group, DateTime now)
		{
			DateTime dt;
			return UpdateTime.TryGetValue(group, out dt) && dt > now;
		}
		static Task<IEnumerable<PropertyGroup>> Dig(Digger digger,Symbol symbol)
		{
			//Console.WriteLine($"start prop dig {symbol} {digger.Name}");
			//return Task.FromResult(Enumerable.Empty<PropertyGroup>());
			return digger.Func(symbol);
		}
		public IObservable<PropertyGroup> Dig(Symbol symbol,Dictionary<string,DateTime> UpdateTime)
		{
			var diggers = new[]
			{ 
				NewDigger("公司概况",DigCompany),
				NewDigger("股本结构",DigCapital),
				NewDigger("十大股东",DigHolder),
				NewDigger("财务摘要",DigAbstr),
				NewDigger("分红信息",DigFinance),
				NewDigger("资产负债",DigLiab),
				NewDigger("利润简表",DigProfit),
				NewDigger("现金流量",DigCash),
			};
			var now = DateTime.Now;
			return from digger in diggers.ToObservable()
				   where !SkipDigger(UpdateTime, digger.Name, now)
				   from groups in Dig(digger,symbol)
				   from g in groups
				   select g;
		}
		public SinaSetting Setting { get; }
		public IHttpClient HttpClient { get; }
		public SinaSymbolPropertyDigger(SinaSetting Setting, IHttpClient HttpClient)
		{
			this.Setting = Setting;
			this.HttpClient = HttpClient;
		}
	}
}
