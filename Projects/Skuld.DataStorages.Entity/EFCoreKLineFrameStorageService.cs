using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using SF;
using SF.Data.Entity;
using SF.Data.Storage;

namespace Skuld.DataStorages.Entity
{
	
	public class EFCoreKLineFrameStorageService  : IKLineFrameStorageService
	{
		public IDataContext Context { get; }
		public EFCoreKLineFrameStorageService(
			IDataContext Context
			)
		{
			this.Context = Context;
		}
		
		class State
		{
			public Dictionary<int, DateTime> EndTimes { get; set; }
		}

		

		public KLineFrameRange GetKLineFrameRequired(Symbol symbol, int Interval)
		{
			DateTime endTime;
			
			var id = symbol.GetIdent();
			endTime = Context.Set<Models.Price>().AsQueryable(true)
				.Where(p => p.Symbol == id && p.Interval == Interval)
				.OrderByDescending(p => p.Time)
				.Take(1)
				.Select(p=>p.Time)
				.SingleOrDefault();
			

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

			var frames=Context.Set<Models.Price>().AsQueryable(true)
				.Where(p => p.Symbol == symbol && p.Interval == Interval && p.Time >= TimeRange.Begin && p.Time <= TimeRange.End)
				.OrderBy(p => p.Time)
				.Select(p => new KLineFrame
				{
					AdjuestRate=p.AdjustRate,
					Close=p.Close,
					High=p.High,
					Low=p.Low,
					Open=p.Open,
					Time=p.Time,
					Volume=p.Volume
				}).ToArray();
				
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

			await Context.Retry(async ct =>
			{
				using (var tran = Context.Engine.BeginTransaction())
				{
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
							await Context.Engine.ExecuteCommandAsync(
								$"INSERT INTO [{setName}] ([symbol], [interval], [time],[open],[close],[high],[low],[volume], [adjustrate]) " +
								"VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8)",
								ct,
								ident,
								Interval,
								f.Time,
								f.Open,
								f.Close,
								f.High,
								f.Low,
								f.Volume,
								f.AdjuestRate
							);
						}
						else if (p.Open != f.Open || p.Close != f.Close || p.High != f.High || p.Low != f.Low ||
							p.Volume != f.Volume || p.AdjustRate != f.AdjuestRate
							)
							await Context.Engine.ExecuteCommandAsync(
								$"update [{setName}] set " +
								"[open]=@p0,[close]=@p1,[high]=@p2,[low]=@p3,[volume]=@p4, [adjustrate]=@p5 " +
								"where [symbol]=@p6 and [interval]=@p7 and [time]=@p8",
								ct,
								f.Open,
								f.Close,
								f.High,
								f.Low,
								f.Volume,
								f.AdjuestRate,
								ident,
								Interval,
								f.Time
								);
					}
					if (ct.IsCancellationRequested)
						tran.Rollback();
					else
						tran.Commit();

				}
				return 0;
			});
		}
	}
}
