using System;

namespace Skuld
{
	public class KLineFrameRange
	{
		public Symbol Symbol { get; set; }
		public int Interval { get; set; }
		public TimeRange TimeRange { get; set; }
	}
}
