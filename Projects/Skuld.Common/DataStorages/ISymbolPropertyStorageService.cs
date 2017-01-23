using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skuld.DataStorages
{
	public interface ISymbolPropertyStorageService
	{
		Task<Dictionary<string, DateTime>> GetNextUpdateTimes(Symbol symbol);
		Task Update(Symbol symbol, IObservable<PropertyGroup> groups);
	}
}
