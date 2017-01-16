using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SF;
using System.Linq;
using System.Data.Entity;

namespace Skuld.DataStorages.Entity
{
	public class EFCoreSymbolCategoryStorageService
	{
		string ConnectionString { get; }
		public EFCoreSymbolCategoryStorageService(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
		}
		async Task EnsureCategoryType(string type)
		{
			await Tasks.Retry(async ct =>
			{
				using (var ctx = new AppContext(ConnectionString))
				{
					var ctype = await ctx.CategoryTypes.FindAsync(type);
					if (ctype == null)
					{
						ctx.CategoryTypes.Add(new Models.CategoryType
						{
							Name = type
						});
						await ctx.SaveChangesAsync();
					}
				}
				return 0;
			});
		}
		async Task EnsureCategory(string type,string name)
		{
			await Tasks.Retry(async ct =>
			{
				using (var ctx = new AppContext(ConnectionString))
				{
					var cat = await ctx.Categories.FindAsync(type,name);
					if (cat == null)
					{
						ctx.Categories.Add(new Models.Category
						{
							Type=type,
							Name = name
						});
						await ctx.SaveChangesAsync();
					}
				}
				return 0;
			});
		}
		async Task EnsureSymbolCategories(string type, string symbol, string[] cats)
		{
			await Tasks.Retry(async ct =>
			{
				using (var ctx = new AppContext(ConnectionString))
				{
					var dics = await ctx
						.CategorySymbols
						.Where(cs => cs.Type == type && cs.Symbol == symbol)
						.ToDictionaryAsync(cs=>cs.Category);

					foreach (var p in dics)
						if (!cats.Any(c => c == p.Key))
							ctx.CategorySymbols.Remove(p.Value);
					foreach (var cat in cats)
						if (!dics.ContainsKey(cat))
							ctx.CategorySymbols.Add(
								new Models.CategorySymbol
								{
									Category = cat,
									Symbol = symbol,
									Type = type
								});
					await ctx.SaveChangesAsync();
				}
				return 0;
			});
		}
		async Task Update(string type, string[] cats,string symbol)
		{
			await EnsureCategoryType(type);
			foreach (var cat in cats)
				await EnsureCategory(type, cat);
			await EnsureSymbolCategories(type, symbol, cats);
		}
		public async Task Update(Symbol symbol,Dictionary<string,string[]> categories)
		{
			foreach (var pair in categories) {
				await Update(pair.Key, pair.Value, symbol.Scope.ScopeCode + symbol.Code);
			}
		}
	}
}
