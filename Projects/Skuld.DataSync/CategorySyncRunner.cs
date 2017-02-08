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
using SF.Core.DI;
using Skuld.DataProviders;
using Skuld.DataStorages;

namespace Skuld.DataSync
{
	class CategorySyncRunner
	{
		public ISymbolCategoryDigger Digger { get; }
		public ISymbolCategoryStorageService StorageService { get; }
		public CategorySyncRunner(ISymbolCategoryDigger Digger,ISymbolCategoryStorageService StorageService)
		{
			this.Digger = Digger;
			this.StorageService = StorageService;

		}
		public async Task Execute(Symbol symbol)
		{
			if (symbol.Scope.Type != SymbolType.Stock)
				return;
			var re = await Digger.Dig(symbol);
			if (re == null)
				return;
			await StorageService.Update(symbol, re);
		}
	}
}