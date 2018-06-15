using SF.Sys;
using SF.Sys.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Skuld.DataStorages.Files
{

	public class FileKLineFrameStorageService 
	{
		public string BasePath { get; }
		public FileKLineFrameStorageService(
			string BasePath
			)
		{
			this.BasePath = BasePath;
			System.IO.Directory.CreateDirectory(BasePath);
		}
		string GetItemBasePath(string Scope, string Code)
		{
			return System.IO.Path.Combine(
				BasePath,
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
					var state = LoadState(s.Scope.ScopeCode, s.Code);
					var end = state.EndTimes?.Get(Interval) ?? new DateTime(1990, 1, 1);
					var now = DateTime.Now;
					return new KLineFrameRange
					{
						Symbol = new Symbol
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
			var fs = GetDataPath(Scope.ScopeCode, Code, Interval);
			var frames = ReadKLineFrames(fs);
			var ob = frames.ToObservable();
			if (TimeRange == null)
				return ob;
			return ob.Where(f => f.Time >= TimeRange.Begin && f.Time < TimeRange.End);
		}

		public async Task Update(SymbolScope Scope, string Code, int Interval, IObservable<KLineFrame> Frames)
		{

			Dictionary<DateTime, KLineFrame> frames = null;
			var fs = GetDataPath(Scope.ScopeCode, Code, Interval);

			await Frames.ForEachAsync(frame =>
			{
				if (frames == null)
					frames = ReadKLineFrames(fs).ToDictionary(k => k.Time);
				frames[frame.Time] = frame;
			});

			if (frames == null)
				return;
			var arr = frames.Values.ToArray();
			Array.Sort(arr, (x, y) => x.Time.CompareTo(y.Time));
			var state = LoadState(Scope.ScopeCode, Code);
			state.EndTimes[Interval] = arr[arr.Length - 1].Time;
			var sb = new StringBuilder();
			foreach (var a in arr)
				sb.AppendLine($"{a.Time},{a.Open},{a.Close},{a.Low},{a.High},{a.Volume:0},{a.AdjuestRate}");
			System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fs));
			System.IO.File.WriteAllText(fs, sb.ToString());
			SaveState(Scope.ScopeCode, Code, state);
		}
	}
}
