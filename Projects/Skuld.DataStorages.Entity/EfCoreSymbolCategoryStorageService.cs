using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SF;
using System.Linq;
using SF.Data.Entity;
using System.Collections.Concurrent;
using SF.Data.Storage;

namespace Skuld.DataStorages.Entity
{
	public class EFCoreSymbolCategoryStorageService : ISymbolCategoryStorageService
	{
		IDataContext Context { get; }
		public EFCoreSymbolCategoryStorageService(IDataContext Context)
		{
			this.Context = Context;
		}
		static ConcurrentDictionary<string, string> TypeNames = new ConcurrentDictionary<string, string>();

		async Task EnsureCategoryType(string type)
		{
			if (TypeNames.ContainsKey(type))
				return;
			await Context.Retry(async ct =>
				await Context.Set<Models.CategoryType>().EnsureAsync(
					new Models.CategoryType
					{
						Name = type
					})
				);
			TypeNames.TryAdd(type, type);
		}
		static ConcurrentDictionary<string, string> CatNames = new ConcurrentDictionary<string, string>();
		async Task EnsureCategory(string type,string name)
		{
			var key = type + ":" + name;
			if (CatNames.ContainsKey(key))
				return;
			await Context.Retry(async ct =>
				await Context.Set<Models.Category>().EnsureAsync(
					new Models.Category
					{
						Type = type,
						Name = name
					})
				);
			CatNames.TryAdd(key, key);
		}

		public async Task Update(Symbol symbol,Dictionary<string,string[]> categories)
		{
			foreach (var p in categories)
			{
				await EnsureCategoryType(p.Key);
				foreach (var cat in p.Value)
					await EnsureCategory(p.Key, cat);
			}

			await Context.Retry(async ct =>
			{
				var set = Context.Set<Models.CategorySymbol>();
				var id = symbol.GetIdent();
				set.Merge(
					await set.QueryAsync(cs => cs.Symbol == id),
					from p in categories
					from v in p.Value
					select new { type = p.Key, cat = v },
					(cs, e) => cs.Category == e.cat && cs.Type==e.type,
					e => new Models.CategorySymbol
					{
						Category = e.cat,
						Symbol = id,
						Type = e.type
					}
					);
				await Context.SaveChangesAsync();
				return 0;
			});
		}
	}
}
