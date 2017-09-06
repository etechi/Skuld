using System;

namespace Skuld
{
	public enum KLineFrameField
	{
		Time,
		Open,
		Close,
		High,
		Low,
		Volume,
		AdjuestRate
	}
	public class KLineFrame
	{
		public DateTime Time { get; set; }
		public float Open { get; set; }
		public float Close { get; set; }
		public float High { get; set; }
		public float Low { get; set; }
		public float Volume { get; set; }
		public float AdjuestRate { get; set; }
		public override string ToString()
		{
			return $"{Time.ToString("yyyy-MM-dd HH:mm:ss")} O:{Open} C:{Close} H:{High} L:{Low} A:{AdjuestRate}";
		}
	}
}
