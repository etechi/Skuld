using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using SF;
using SF.Sys.Linq;
using SF.Sys.Data;
using System.Data.Common;
using Dapper;
using System.Threading;

namespace Skuld.DataStorages.Entity
{
	
	public class EFCoreKLineFrameStorageService  : IKLineFrameStorageService
	{
		public IDataScope DataScope { get; }
		public EFCoreKLineFrameStorageService(
			IDataScope DataScope
			)
		{
			this.DataScope = DataScope;
		}
		
		class State
		{
			public Dictionary<int, DateTime> EndTimes { get; set; }
		}

		

		public KLineFrameRange GetKLineFrameRequired(Symbol symbol, int Interval)
		{
			DateTime endTime;
			
			var id = symbol.GetIdent();
			endTime = DataScope.Use("结束时间", Context =>
				Context.Queryable<Models.Price>()
				.Where(p => p.Symbol == id && p.Interval == Interval)
				.OrderByDescending(p => p.Time)
				.Take(1)
				.Select(p => p.Time)
				.SingleOrDefaultAsync()
				).Result;
			

			var end = endTime==default(DateTime)?new DateTime(1990, 1, 1):endTime;
			var now = DateTime.Now;
			return new KLineFrameRange
			{
				Symbol=new Symbol
				{
					Code = symbol.Code,
					Scope = symbol.Scope
				},
				Interval = Interval,
				TimeRange = new TimeRange
				{
					Begin = now.Subtract(end).TotalMinutes < Interval ? now : end,
					End = now
				}
			};
		}

		
		public IObservable<KLineFrame> Load(SymbolScope Scope, string Code, int Interval, TimeRange TimeRange)
		{
			var symbol = new Symbol { Scope = Scope, Code = Code }.GetIdent();

			var frames = DataScope.Use("载入项目",
				Context =>
					Context.Queryable<Models.Price>()
					.Where(p => p.Symbol == symbol && p.Interval == Interval && p.Time >= TimeRange.Begin && p.Time <= TimeRange.End)
					.OrderBy(p => p.Time)
					.Select(p => new KLineFrame
					{
						AdjuestRate = p.AdjustRate,
						Close = p.Close,
						High = p.High,
						Low = p.Low,
						Open = p.Open,
						Time = p.Time,
						Volume = p.Volume
					}).ToArrayAsync()
				).Result;
				
				
			return frames.ToObservable();
		}

		public async Task UpdateAsync(Symbol Symbol, int Interval, IObservable<KLineFrame> Frames)
		{
			var frames = new Dictionary<DateTime, KLineFrame>();
			var minTime = DateTime.MaxValue;
			await Frames.ForEachAsync(frame =>
			{
				frames[frame.Time] = frame;
				if (frame.Time < minTime)
					minTime = frame.Time;
			});

			if (frames.Count == 0)
				return;

			var ident = Symbol.GetIdent();

			await DataScope.Retry("增加报价",async Context =>
			{
				var ctxex = (IDataContextExtension)Context;
				var set = Context.Set<Models.Price>();
				var existsframes = await set.AsQueryable(true)
					.Where(p => p.Symbol == ident && p.Interval == Interval && p.Time >= minTime)
					.ToDictionaryAsync(p => p.Time);
				var setName = set.Metadata.EntitySetName;
				foreach (var f in frames.Values)
				{
					Models.Price p;
					if (!existsframes.TryGetValue(f.Time, out p))
					{
						await ctxex.GetDbConnection().ExecuteAsync(
							$"INSERT INTO [{setName}] ([symbol], [interval], [time],[open],[close],[high],[low],[volume], [adjustrate]) " +
							"VALUES(@symbol,@interval,@time,@open,@close,@high,@low,@volume,@adjustrate)",
							new
							{
								symbol = ident,
								interval = Interval,
								time = f.Time,
								open = f.Open,
								close = f.Close,
								high = f.High,
								low = f.Low,
								volume = f.Volume,
								adjustrate = f.AdjuestRate
							},
							(DbTransaction)ctxex.Transaction?.RawTransaction
						);
					}
					else if (p.Open != f.Open || p.Close != f.Close || p.High != f.High || p.Low != f.Low ||
						p.Volume != f.Volume || p.AdjustRate != f.AdjuestRate
						)
						await ctxex.GetDbConnection().ExecuteAsync(
							$"update [{setName}] set " +
							"[open]=@open,[close]=@close,[high]=@high,[low]=@low,[volume]=@volume, [adjustrate]=@adjustrate " +
							"where [symbol]=@symbol and [interval]=@interval and [time]=@time",
							new
							{
								open = f.Open,
								close = f.Close,
								high = f.High,
								low = f.Low,
								volume = f.Volume,
								adjustrate = f.AdjuestRate,
								symbol = ident,
								interval = Interval,
								time = f.Time
							},
							(DbTransaction)ctxex.Transaction?.RawTransaction
							);
				}
				return 0;
			},IsolationLevel:System.Data.IsolationLevel.ReadCommitted);
		}
	}
}
