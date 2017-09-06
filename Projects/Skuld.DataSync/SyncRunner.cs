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
using SF.Core.DI;
using Skuld.DataProviders;
using Skuld.DataStorages;
using SF.Core.ServiceManagement;

namespace Skuld.DataSync
{
	class SyncRunner
	{
		public IServiceScopeFactory ScopeFactory { get; }
		public SyncRunner(IServiceScopeFactory ScopeFactory)
		{
			this.ScopeFactory = ScopeFactory;

		}

		public async Task Execute()
		{
			var fc = new FlowController(40);
			Symbol[] symbols;
			using (var s = ScopeFactory.CreateServiceScope())
			{
				await s.ServiceProvider.Resolve<SymbolSyncRunner>().Execute();
				symbols = await s.ServiceProvider.Resolve<ISymbolStorageService>().List(null).ToAsyncEnumerable().ToArray();
			}

			await symbols.ToObservable()
				.Delay(r => Observable.FromAsync(fc.Wait).IgnoreElements())
				.SelectMany(
					async (symbol) =>
					{
						try
						{
							using (var s = ScopeFactory.CreateServiceScope())
							{
								Console.WriteLine($"dig {symbol} {fc.CurrentThreadCount}/{fc.WaitingCount}");
								await s.ServiceProvider.Resolve<PriceSyncRunner>().Execute(symbol);
								await s.ServiceProvider.Resolve<CategorySyncRunner>().Execute(symbol);
								await s.ServiceProvider.Resolve<PropertySyncRunner>().Execute(symbol);
								Console.WriteLine($"dig {symbol} done!");
							}
						}
						catch (Exception e)
						{
							Console.WriteLine($"dig {symbol} error {e.Message}!");
						}
						finally
						{
							fc.Complete();
						}
						return 0;
					})
				.ForEachAsync(o => { });
		}
	}
}