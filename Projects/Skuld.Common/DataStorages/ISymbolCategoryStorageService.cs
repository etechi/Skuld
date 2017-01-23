using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skuld.DataStorages
{
	public interface ISymbolCategoryStorageService
	{
		Task Update(Symbol symbol, Dictionary<string, string[]> categories);
	}
}
