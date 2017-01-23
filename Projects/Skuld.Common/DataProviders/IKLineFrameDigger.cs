using System;

namespace Skuld.DataProviders
{
	public interface IKLineFrameDigger
	{
		IObservable<KLineFrame> Dig(
			Symbol Symbol,
			int TimeInterval,
			DateTime BeginTime,
			DateTime EndTime
		   );
	}
}
