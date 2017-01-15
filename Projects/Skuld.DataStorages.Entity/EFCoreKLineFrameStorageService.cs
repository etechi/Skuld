using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using SF;
using System.Data.Entity;

namespace Skuld.DataStorage.EFCore
{
	
	public class EFCoreKLineFrameStorageService 
	{
		public string ConnectionString { get; }
		public EFCoreKLineFrameStorageService(
			string ConnectionString
			)
		{
			this.ConnectionString = ConnectionString;
		}
		string GetItemBasePath(string Scope, string Code)
		{
			return System.IO.Path.Combine(
				ConnectionString,
				Scope,
				Code.Substring(0, Math.Max(1, Code.Length / 2)),
				Code
				);
		}
		string GetDataPath(string Scope, string Code, int Interval)
		{
			return System.IO.Path.Combine(
				GetItemBasePath(Scope, Code),
				$"{Scope}-{Code}-{Interval}.csv"
				);
		}
		string GetStatePath(string Scope, string Code)
		{
			return System.IO.Path.Combine(
				GetItemBasePath(Scope, Code),
				$"{Scope}-{Code}.json"
				);
		}

		class State
		{
			public Dictionary<int, DateTime> EndTimes { get; set; }
		}

		State LoadState(string Scope, string Code)
		{
			var sf = GetStatePath(Scope, Code);
			if (System.IO.File.Exists(sf))
				return Json.Parse<State>(System.IO.File.ReadAllText(sf));
			return new State { EndTimes = new Dictionary<int, DateTime>() };
		}
		void SaveState(string Scope, string Code, State State)
		{
			var sf = GetStatePath(Scope, Code);
			System.IO.File.WriteAllText(sf, Json.Stringify(State));
		}

		public IObservable<KLineFrameRange> GetKLineFrameRequired(IObservable<Symbol> symbols, int Interval)
		{
			return symbols
				.Select(s => {
					DateTime endTime;
					using (var ctx = new AppContext(ConnectionString))
					{
						var id = s.GetIdent();
						endTime = ctx.Prices
							.Where(p => p.Symbol == id && p.Interval == Interval)
							.OrderByDescending(p => p.Time)
							.Take(1)
							.Select(p=>p.Time)
							.SingleOrDefault();
					}

					var end = endTime==default(DateTime)?new DateTime(1990, 1, 1):endTime;
					var now = DateTime.Now;
					return new KLineFrameRange
					{
						Symbol=new Symbol
						{
							Code = s.Code,
							Scope = s.Scope
						},
						Interval = Interval,
						TimeRange = new TimeRange
						{
							Begin = now.Subtract(end).TotalMinutes < Interval ? now : end,
							End = now
						}
					};
				});
		}

		IEnumerable<KLineFrame> ReadKLineFrames(string Path)
		{
			if (!System.IO.File.Exists(Path))
				yield break;
			using (var fs = new System.IO.FileStream(Path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
			{
				using (var r = new System.IO.StreamReader(fs))
				{
					for (;;)
					{
						var l = r.ReadLine();
						if (l == null)
							break;
						l = l.Trim();
						if (l.Length == 0)
							continue;
						var ts = l.Split(',');
						yield return new KLineFrame
						{
							Time = DateTime.Parse(ts[0]),
							Open = float.Parse(ts[1]),
							Close = float.Parse(ts[2]),
							Low = float.Parse(ts[3]),
							High = float.Parse(ts[4]),
							Volume = float.Parse(ts[5]),
							AdjuestRate = float.Parse(ts[6])
						};
					}
				}
			}
		}
		public IObservable<KLineFrame> Load(SymbolScope Scope, string Code, int Interval, TimeRange TimeRange)
		{
			KLineFrame[] frames;
			using (var ctx = new AppContext(ConnectionString))
			{
				var symbol = new Symbol { Scope = Scope, Code = Code }.GetIdent();

				frames=ctx.Prices
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
			}	
			return frames.ToObservable();
		}

		public async Task Update(Symbol Symbol, int Interval, IObservable<KLineFrame> Frames)
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

			await Tasks.Retry(async ct =>
			{
				using (var ctx = new AppContext(ConnectionString))
				{
					using (var tran = ctx.Database.BeginTransaction())
					{
						var existsframes = await ctx.Prices
							.Where(p => p.Symbol == ident && p.Interval == Interval && p.Time >= minTime)
							.AsNoTracking()
							.ToDictionaryAsync(p => p.Time);
						foreach (var f in frames.Values)
						{
							Models.Price p;
							if (!existsframes.TryGetValue(f.Time, out p))
							{
								await ctx.Database.ExecuteSqlCommandAsync(
									"INSERT INTO [prices] ([symbol], [interval], [time],[open],[close],[high],[low],[volume], [adjustrate]) " +
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
								await ctx.Database.ExecuteSqlCommandAsync(
									"update [prices] set " +
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
				}
				return 0;
			});
		}
	}
}
