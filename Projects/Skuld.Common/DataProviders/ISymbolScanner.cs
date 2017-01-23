using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skuld.DataProviders
{
	public interface ISymbolScanner
	{
		IObservable<Symbol> Scan();
	}
}
