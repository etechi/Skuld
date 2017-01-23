using System;
using System.Threading.Tasks;

namespace Skuld.DataStorages
{
	public interface ISymbolStorageService
	{
		IObservable<Symbol> List(SymbolScope Scope);

		Task Update(IObservable<Symbol> symbols);
	}
}
