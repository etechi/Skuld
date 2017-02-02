using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using SF;
using System.Text.RegularExpressions;
using System.Text;

namespace Skuld.DataProviders.Sina
{
	public class SinaSymbolCategoryDigger : DataProviders.ISymbolCategoryDigger
	{
		static Regex RegHtmlClear = new Regex("<[^>]+>");
		public async Task<Dictionary<string,string[]>> Dig(Symbol symbol)
		{
			var args = new Dictionary<string, string> {
				{"SYMBOL",symbol.Scope.ScopeCode+symbol.Code }
				};
			var url = new Uri(SimpleTemplate.Eval(Setting.CategoryUrl, args));
			var html=await url.GetString();
			var start = html.IndexOf("热点板块");
			if (start == -1)
				return new Dictionary<string, string[]>();
			start += 4;
			var end = html.IndexOf("实时行情", start);
			if (end == -1)
				return new Dictionary<string, string[]>();
			var content = html.Substring(start, end - start)
				.Replace("<br/>","\n")
				.Replace("&nbsp;"," ");
			content = RegHtmlClear.Replace(content, "");
			return (
				from line in content.SplitAndNormalizae('\n')
				let cat = line.Split2(':')
				where cat.Item2.Length>0
				let type = cat.Item1
				let items = cat.Item2.SplitAndNormalizae(' ').ToArray()
				select new { type = type, items = items }
				).ToDictionary(i => i.type, i => i.items);
		}
		public SinaSetting Setting { get; }
		public SinaSymbolCategoryDigger(SinaSetting Setting)
		{
			this.Setting = Setting;
		}
	}
}
