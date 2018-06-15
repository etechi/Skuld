using System.Threading.Tasks;
using Skuld.DataProviders;
using Skuld.DataStorages;

namespace Skuld.DataSync
{
	class PriceSyncRunner
	{
		public IKLineFrameDigger Digger { get; }
		public IKLineFrameStorageService StorageService { get; }
		public PriceSyncRunner(IKLineFrameDigger Digger, IKLineFrameStorageService StorageService)
		{
			this.Digger = Digger;
			this.StorageService = StorageService;
		}
		public async Task Execute(Symbol symbol)
		{
			var range = StorageService.GetKLineFrameRequired(
				symbol,
				TimeIntervals.Day
				);
			if (range.TimeRange.Begin >= range.TimeRange.End)
				return;
			var lines = Digger.Dig(
				range.Symbol,
				TimeIntervals.Day,
				range.TimeRange.Begin,
				range.TimeRange.End
				);
			await StorageService.UpdateAsync(
				range.Symbol,
				TimeIntervals.Day,
				lines
				);
		}
	}
}