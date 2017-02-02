using System;
using Skuld.DataProviders.Sina;
using Skuld.DataStorages.Entity;
using System.Threading.Tasks;
using Skuld;
using System.Reactive.Linq;
using System.IO;
using System.Reactive.Joins;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SF.Data.Entity.EntityFrameworkCore;
using SF.Core.DI;
using Skuld.DataProviders;
using Skuld.DataStorages;

namespace Skuld.DataSync
{

	class PropertySyncRunner
	{
		public ISymbolPropertyDigger Digger { get; }
		public ISymbolPropertyStorageService StorageService { get; }
		public PropertySyncRunner(ISymbolPropertyDigger Digger, ISymbolPropertyStorageService StorageService)
		{
			this.Digger = Digger;
			this.StorageService = StorageService;
		}
		public async Task Execute(Symbol symbol)
		{
			if (symbol.Scope.Type != SymbolType.Stock)
				return;
			var updateTimes = await StorageService.GetNextUpdateTimes(symbol);
			var re = Digger.Dig(symbol, updateTimes);
			await StorageService.Update(symbol, re);
		}
	}
}