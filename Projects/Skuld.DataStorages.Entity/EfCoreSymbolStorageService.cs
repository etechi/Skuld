using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SF;
using System.Linq;
using SF.Sys.Data;

namespace Skuld.DataStorages.Entity
{
	public class EFCoreSymbolStorageService : ISymbolStorageService
	{
		IDataScope DataScope { get; }
		public EFCoreSymbolStorageService(IDataScope DataScope)
		{
			this.DataScope = DataScope;
		}
		Task<Symbol[]> LoadSymbols(SymbolScope Scope)
		{
			return DataScope.Use("载入项目", Context =>
			{
				var q = Context.Queryable<Models.Symbol>();
				if (Scope != null)
					q = q.Where(s => s.Type == Scope.Type && s.ScopeCode == Scope.ScopeCode).OrderBy(s => s.Code);
				return q
					.Select(s => new Symbol
					{
						Scope = new SymbolScope
						{
							ScopeCode = s.ScopeCode,
							Type = s.Type
						},
						Code = s.Code,
						Name = s.Name
					})
					.ToArrayAsync();
			});
		}
		public IObservable<Symbol> List(SymbolScope Scope)
		{
			return Observable.FromAsync(()=>LoadSymbols(Scope)).SelectMany(s => s);
		}

		public Task Update(IObservable<Symbol> symbols)
		{
			return DataScope.Use("更新项目", async Context =>
			{
				var set = Context.Set<Models.Symbol>();
				var exists = await set.AsQueryable().ToDictionaryAsync(s => s.Id);

				var newSymbols = new Dictionary<string, Symbol>();
				var changedSymbols = new Dictionary<string, Symbol>();

				await symbols.ForEachAsync(s =>
				{
					var id = s.GetIdent();
					Models.Symbol es;
					if (!exists.TryGetValue(id, out es))
						newSymbols[id] = s;
					else if (s.Name != es.Name)
						changedSymbols[id] = s;
				});


				foreach (var s in newSymbols.Values)
					set.Add(new Models.Symbol
					{
						Code = s.Code,
						Id = s.GetIdent(),
						Name = s.Name,
						ScopeCode = s.Scope.ScopeCode,
						Type = s.Scope.Type
					});

				foreach (var s in changedSymbols.Values)
				{
					var e = await set.FindAsync(s.GetIdent());
					if (e == null)
						set.Add(new Models.Symbol
						{
							Code = s.Code,
							Id = s.GetIdent(),
							Name = s.Name,
							ScopeCode = s.Scope.ScopeCode,
							Type = s.Scope.Type
						});
					else if (e.Name != s.Name)
					{
						e.Name = s.Name;
						set.Update(e);
					}
				}
				await Context.SaveChangesAsync();
			});
		}
	}
}
