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
using SF.DI;
using Skuld.DataProviders;
using Skuld.DataStorages;

namespace Skuld.DataSync
{
	class SymbolSyncRunner
	{
		public ISymbolScanner Scanner { get; }
		public ISymbolStorageService StorageService { get; }
		public SymbolSyncRunner(ISymbolScanner Scanner, ISymbolStorageService StorageService)
		{
			this.Scanner = Scanner;
			this.StorageService = StorageService;
		}
		public async Task Execute()
		{
			var ss = Scanner.Scan();
			await StorageService.Update(ss);
		}
	}
}