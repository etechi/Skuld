using System;
using Skuld.DataProvider.Sina;
using Skuld.DataStorage.Files;
using System.Threading.Tasks;
using Skuld;
using System.Reactive.Linq;
using System.IO;

class Program
{
	static async Task Sync(string BasePath)
	{
		var SinaSetting = new SinaSetting();
		var SymbolScanner = new SinaSymbolScanner(SinaSetting);
		var SymbolStorageService = new FileSymbolStorageService(Path.Combine(BasePath,"symbols"));
		var KLineFrameStorageService = new FileKLineFrameStorageService(Path.Combine(BasePath,"klines"));
		var KLineFrameDigger = new SinaKLineFrameDigger(SinaSetting);

		var ss = SymbolScanner.Scan();
		await SymbolStorageService.Update(ss);

		await KLineFrameStorageService.GetKLineFrameRequired(
			SymbolStorageService.List(null),
			TimeIntervals.Day
			)
			.Where(r => r.TimeRange.Begin < r.TimeRange.End)
			.Select((r, i) => new { range = r, i = i })
			.Delay(r => Observable.Timer(TimeSpan.FromMilliseconds(r.i * 2*100)))
			.ForEachAsync(r =>
				Task.Run(
					async () =>
					{
						Console.WriteLine($"dig {r.range.Scope}-{r.range.Code}");
						var lines = KLineFrameDigger.Dig(

							r.range.Scope,
							r.range.Code,
							TimeIntervals.Day,
							r.range.TimeRange.Begin,
							r.range.TimeRange.End
							);
						await KLineFrameStorageService.Update(
							r.range.Scope,
							r.range.Code,
							TimeIntervals.Day,
							lines
							);
					}
					));
	}
	static void Main(string[] args)
    {
		Sync(args[0]).Wait();

    }
}