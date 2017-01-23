using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skuld.DataProviders
{
	public interface ISymbolCategoryDigger
	{
		Task<Dictionary<string, string[]>> Dig(Symbol symbol);
	}
}
