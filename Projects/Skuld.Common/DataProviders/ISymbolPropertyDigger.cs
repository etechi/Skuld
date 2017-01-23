using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skuld.DataProviders
{
	public interface ISymbolPropertyDigger
	{
		IObservable<PropertyGroup> Dig(Symbol symbol, Dictionary<string, DateTime> UpdateTime);
	}
}
