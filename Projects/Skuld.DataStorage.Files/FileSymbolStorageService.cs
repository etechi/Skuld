using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Skuld.DataStorage.Files
{
	public class FileSymbolStorageService
	{
		string BasePath { get; }
		public FileSymbolStorageService(string BasePath)
		{
			this.BasePath = BasePath;
			System.IO.Directory.CreateDirectory(BasePath);
		}
		Dictionary<string, Symbol> LoadSymbols(string Scope)
		{
			var path = System.IO.Path.Combine(BasePath, Scope + ".json");
			return System.IO.File.Exists(path) ?
				Json.Parse<Dictionary<string, Symbol>>(System.IO.File.ReadAllText(path)) :
				new Dictionary<string, Symbol>();
		}
		void SaveSymbols(string Scope, Dictionary<string, Symbol> Symbols)
		{
			var path = System.IO.Path.Combine(BasePath, Scope + ".json");
			System.IO.File.WriteAllText(path, Json.Stringify(Symbols));
		}
		public IObservable<Symbol> List(SymbolScope Scope)
		{
			if (Scope == null)
			{
				return from file in System.IO.Directory.GetFiles(BasePath, "*.json").ToObservable()
					   let dic = LoadSymbols(System.IO.Path.GetFileNameWithoutExtension(file))
					   from s in dic.Values
					   select s;
			}
			else
			{
				var dic = LoadSymbols(Scope.ToString());
				return dic.Values.ToObservable();
			}
		}

		public async Task Update(IObservable<Symbol> symbols)
		{
			var existSymbols = new Dictionary<string, Dictionary<string, Symbol>>();
			var changedScopes = new Dictionary<string, Dictionary<string, Symbol>>();
			await symbols.ForEachAsync(s => {
				var scope = s.Scope.ToString();
				existSymbols.TryGetValue(scope,out var ess);
				if (ess == null)
				{
					ess = LoadSymbols(scope);
					existSymbols[scope] = ess;
				}

				if (!ess.TryGetValue(s.Code,out var _))
				{
					changedScopes[scope] = ess;
					ess[s.Code] = s;
				}
			});
			foreach (var pair in changedScopes)
				SaveSymbols(pair.Key, pair.Value);
		}
	}
}
