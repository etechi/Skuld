using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SF;
using System.Linq;
using SF.Sys.Data;
using System.Collections.Concurrent;

namespace Skuld.DataStorages.Entity
{
	public class EFCoreSymbolCategoryStorageService : ISymbolCategoryStorageService
	{
		IDataScope DataScope { get; }
		public EFCoreSymbolCategoryStorageService(IDataScope DataScope)
		{
			this.DataScope= DataScope;
		}
		static ConcurrentDictionary<string, string> TypeNames = new ConcurrentDictionary<string, string>();

		async Task EnsureCategoryType(string type)
		{
			if (TypeNames.ContainsKey(type))
				return;
			await DataScope.Retry("新增分类类型",Context=>
				Context.Set<Models.CategoryType>().EnsureAsync(
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
			await DataScope.Retry("新增分类",Context=>
				Context.Set<Models.Category>().EnsureAsync(
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

			await DataScope.Retry("新增项目", async Context =>
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
