using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SF;
using System.Linq;
namespace Skuld.DataStorages.Files
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
			var path = System.IO.Path.Combine(BasePath, Scope + ".csv");
			if (!System.IO.File.Exists(path))
				return new Dictionary<string, Symbol>();

			return (from l in System.IO.File.ReadAllLines(path)
					let fs= l.SplitAndNormalizae(',').ToArray()
					select new Symbol
					{
						Scope = new SymbolScope { Type = (SymbolType)Enum.Parse(typeof(SymbolType), fs[1]),ScopeCode=fs[2] },
						Code=fs[0],
						Name=fs[3]
					})
				.ToDictionary(s => s.Code);
		}
		void SaveSymbols(string Scope, Dictionary<string, Symbol> Symbols)
		{
			var path = System.IO.Path.Combine(BasePath, Scope + ".csv");

			System.IO.File.WriteAllText(
				path, 
				Symbols.Values.Select(s=>$"{s.Code},{s.Scope.Type},{s.Scope.ScopeCode},{s.Name}").Join("\n")
				);
		}
		public IObservable<Symbol> List(SymbolScope Scope)
		{
			if (Scope == null)
			{
				return from file in System.IO.Directory.GetFiles(BasePath, "*.csv").ToObservable()
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
				Dictionary<string, Symbol> ess;
				existSymbols.TryGetValue(scope,out ess);
				if (ess == null)
				{
					ess = LoadSymbols(scope);
					existSymbols[scope] = ess;
				}

				if (!ess.ContainsKey(s.Code))
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
