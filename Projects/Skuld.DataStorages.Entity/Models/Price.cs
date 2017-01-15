using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorage.EFCore.Models
{
	public class Price
	{
		[Key]
		[Column(Order =1)]
		[MaxLength(20)]
		public string Symbol { get; set; }

		[Key]
		[Column(Order=2)]
		public int Interval { get; set; }

		[Key]
		[Column(Order = 3)]
		public DateTime Time { get; set; }

		public float Open { get; set; }
		public float Close { get; set; }
		public float High { get; set; }
		public float Low { get; set; }
		public float Volume { get; set; }
		public float AdjustRate { get; set; }
    }
}
