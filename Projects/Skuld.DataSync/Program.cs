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

class Program
{
	static string connString = "name=skuld";
	static SinaSetting SinaSetting = new SinaSetting();

	static async Task DigPrice(Symbol symbol)
	{
		var KLineFrameStorageService = new EFCoreKLineFrameStorageService(connString);
		var KLineFrameDigger = new SinaKLineFrameDigger(SinaSetting);
		var range = KLineFrameStorageService.GetKLineFrameRequired(
			symbol,
			TimeIntervals.Day
			);
		if (range.TimeRange.Begin >= range.TimeRange.End)
			return;
		var lines = KLineFrameDigger.Dig(
			range.Symbol,
			TimeIntervals.Day,
			range.TimeRange.Begin,
			range.TimeRange.End
			);
		await KLineFrameStorageService.Update(
			range.Symbol,
			TimeIntervals.Day,
			lines
			);
	}

	static async Task DigCategory(Symbol symbol)
	{
		if (symbol.Scope.Type != SymbolType.Stock)
			return;
		var CatStorageService = new EFCoreSymbolCategoryStorageService(connString);
		var CatDigger = new SinaSymbolCategoryDigger(SinaSetting);

		var re=await CatDigger.Dig(symbol);
		if (re == null)
			return;
		await CatStorageService.Update(symbol, re);
	}
	static async Task DigProperty(Symbol symbol)
	{
		if (symbol.Scope.Type != SymbolType.Stock)
			return;
		var PropStorageService = new EFCoreSymbolPropertyStorageService(connString);
		var PropDigger = new SinaSymbolPropertyDigger(SinaSetting);

		var updateTimes = await PropStorageService.GetNextUpdateTimes(symbol);
		var re = PropDigger.Dig(symbol, updateTimes);
		await PropStorageService.Update(symbol, re);
	}
	static async Task Sync()
	{
		var SymbolScanner = new SinaSymbolScanner(SinaSetting);
		var SymbolStorageService = new EFCoreSymbolStorageService(connString);
		var fc = new SF.FlowController(40,TimeSpan.FromMilliseconds(100));

		var ss = SymbolScanner.Scan();
		await SymbolStorageService.Update(ss);

		await SymbolStorageService.List(null)
			.Select((s, i) => new { s = s, i = i })
			.Delay(r => fc.Next(r.i))
			.Select(s=>s.s)
			.SelectMany(
				async (symbol) =>
				{
					try
					{
						Console.WriteLine($"dig {symbol} {fc.CurrentThreadCount}/{fc.WaitingCount}");
						await DigPrice(symbol);
						await DigCategory(symbol);
						await DigProperty(symbol);
						Console.WriteLine($"dig {symbol} done!");
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
	static void Main(string[] args)
    {
		Sync().Wait();

    }
}