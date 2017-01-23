using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SF;
using System.Linq;
using SF.Data.Entity;

namespace Skuld.DataStorages.Entity
{
	public class EFCoreSymbolCategoryStorageService : ISymbolCategoryStorageService
	{
		IDataContext Context { get; }
		public EFCoreSymbolCategoryStorageService(IDataContext Context)
		{
			this.Context = Context;
		}
		async Task EnsureCategoryType(string type)
		{
			await Context.Retry(async ct =>
				await Context.Set<Models.CategoryType>().EnsureAsync(
					new Models.CategoryType
					{
						Name = type
					})
				);
		}
		async Task EnsureCategory(string type,string name)
		{
			await Context.Retry(async ct =>
				await Context.Set<Models.Category>().EnsureAsync(
					new Models.Category
					{
						Type = type,
						Name = name
					})
				);
		}
		async Task EnsureSymbolCategories(string type, string symbol, string[] cats)
		{
			await Context.Retry(async ct =>
			{
				var set = Context.Set<Models.CategorySymbol>();
				set.Merge(
					await set.LoadListAsync(cs => cs.Type == type && cs.Symbol == symbol),
					cats,
					(cs, e) => cs.Category == e,
					e => new Models.CategorySymbol
					{
						Category = e,
						Symbol = symbol,
						Type = type
					}
					);
				await Context.SaveChangesAsync();
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
