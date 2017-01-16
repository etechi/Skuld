using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SF;
using System.Linq;
using System.Data.Entity;

namespace Skuld.DataStorages.Entity
{
	public class EFCoreSymbolStorageService
	{
		string ConnectionString { get; }
		public EFCoreSymbolStorageService(string ConnectionString)
		{
			this.ConnectionString = ConnectionString;
		}
		async Task<Symbol[]> LoadSymbols(SymbolScope Scope)
		{
			using(var ctx=new AppContext(ConnectionString))
			{
				IQueryable<Models.Symbol> q = ctx.Symbols;
				if (Scope != null)
					q = q.Where(s => s.Type == Scope.Type && s.ScopeCode == Scope.ScopeCode).OrderBy(s => s.Code);
				return await q
					.Select(s => new Symbol
					{
						Scope = new SymbolScope
						{
							ScopeCode=s.ScopeCode,
							Type=s.Type
						},
						Code = s.Code,
						Name = s.Name
					})
					.AsNoTracking()
					.ToArrayAsync();
			}
		}
		public IObservable<Symbol> List(SymbolScope Scope)
		{
			return Observable.FromAsync(()=>LoadSymbols(Scope)).SelectMany(s => s);
		}

		public async Task Update(IObservable<Symbol> symbols)
		{
			Dictionary<string, Models.Symbol> exists = null;
			using (var ctx = new AppContext(ConnectionString))
			{
				exists = await ctx.Symbols.AsNoTracking().ToDictionaryAsync(s => s.Id);
			}

			var newSymbols = new Dictionary<string, Symbol>();
			var changedSymbols = new Dictionary<string, Symbol>();

			await symbols.ForEachAsync(s => {
				var id = s.GetIdent();
				Models.Symbol es;
				if (!exists.TryGetValue(id, out es))
					newSymbols[id] = s;
				else if(s.Name!=es.Name)
					changedSymbols[id] = s;
			});

			using (var ctx = new AppContext(ConnectionString))
			{
				foreach (var s in newSymbols.Values)
					ctx.Symbols.Add(new Models.Symbol
					{
						Code = s.Code,
						Id = s.GetIdent(),
						Name = s.Name,
						ScopeCode = s.Scope.ScopeCode,
						Type = s.Scope.Type
					});

				foreach(var s in changedSymbols.Values)
				{
					var e = await ctx.Symbols.FindAsync(s.GetIdent());
					if(e==null)
						ctx.Symbols.Add(new Models.Symbol
						{
							Code = s.Code,
							Id = s.GetIdent(),
							Name = s.Name,
							ScopeCode = s.Scope.ScopeCode,
							Type = s.Scope.Type
						});
					else if(e.Name!=s.Name)
					{
						e.Name = s.Name;
						ctx.Entry(e).State=System.Data.Entity.EntityState.Modified;
					}
				}
				await ctx.SaveChangesAsync();
			}
		}
	}
}
