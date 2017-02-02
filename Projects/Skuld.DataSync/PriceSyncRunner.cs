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