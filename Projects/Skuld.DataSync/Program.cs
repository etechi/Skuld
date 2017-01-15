using System;
using Skuld.DataProvider.Sina;
using Skuld.DataStorage.EFCore;
using System.Threading.Tasks;
using Skuld;
using System.Reactive.Linq;
using System.IO;
using System.Reactive.Joins;
using System.Collections.Generic;
using System.Reactive.Disposables;

class Program
{
	
	static async Task Sync()
	{
		var connString = "name=skuld";
		var SinaSetting = new SinaSetting();
		var SymbolScanner = new SinaSymbolScanner(SinaSetting);
		var SymbolStorageService = new EFCoreSymbolStorageService(connString);
		var KLineFrameStorageService = new EFCoreKLineFrameStorageService(connString);
		var KLineFrameDigger = new SinaKLineFrameDigger(SinaSetting);
		var fc = new SF.FlowController(40,TimeSpan.FromMilliseconds(100));

		var ss = SymbolScanner.Scan();
		await SymbolStorageService.Update(ss);


		await KLineFrameStorageService.GetKLineFrameRequired(
			SymbolStorageService.List(null),
			TimeIntervals.Day
			)
			.Where(r => r.TimeRange.Begin < r.TimeRange.End)
			.Select((r, i) => new { range = r, i = i })
			.Delay(r => fc.Next(r.i))
			.SelectMany(
				async (r) =>
				{
					try
					{
						Console.WriteLine($"dig {r.range.Symbol} {fc.CurrentThreadCount}/{fc.WaitingCount}");
						var lines = KLineFrameDigger.Dig(
							r.range.Symbol,
							TimeIntervals.Day,
							r.range.TimeRange.Begin,
							r.range.TimeRange.End
							);
						await KLineFrameStorageService.Update(
							r.range.Symbol,
							TimeIntervals.Day,
							lines
							);
						Console.WriteLine($"dig {r.range.Symbol} done!");
					}
					catch (Exception e)
					{
						Console.WriteLine($"dig {r.range.Symbol} error {e.Message}!");
					}
					finally
					{
						fc.Complete();
					}
					return 0;
				}).ForEachAsync(o => { });

	}
	static void Main(string[] args)
    {
		Sync().Wait();

    }
}