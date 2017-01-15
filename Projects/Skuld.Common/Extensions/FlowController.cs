using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF
{
	public class FlowController
	{
		public int ThreadCount { get; }
		int _CurrentThreadCount;
		public int CurrentThreadCount => _CurrentThreadCount;
		public int WaitingCount => WaitQueue.Count;
		Queue<Action> WaitQueue { get; } = new Queue<Action>();
		public TimeSpan UnitTime { get; }
		public FlowController(int ThreadCount, TimeSpan UnitTime)
		{
			this.ThreadCount = ThreadCount;
			this.UnitTime = UnitTime;
		}
		public IObservable<TimeSpan> Next(int index)
		{
			lock (WaitQueue)
			{
				if (_CurrentThreadCount < ThreadCount)
				{
					_CurrentThreadCount++;
					return new TimeSpan[] { }.ToObservable();
				}
				return Observable.Create<TimeSpan>(o =>
				{
					lock (WaitQueue)
					{
						WaitQueue.Enqueue(() =>
						{
							//o.OnNext(new TimeSpan(UnitTime.Ticks * index));
							o.OnCompleted();
						});
						return Disposable.Empty;
					}
				});
			}
		}
		public void Complete()
		{
			lock (WaitQueue)
			{
				if (WaitQueue.Count > 0)
				{
					var o = WaitQueue.Dequeue();
					o();
					return;
				}
				_CurrentThreadCount--;
			}
		}
	}
}
