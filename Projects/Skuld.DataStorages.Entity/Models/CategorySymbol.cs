using SF.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorages.Entity.Models
{
	[Table("CategorySymbols")]
	public class CategorySymbol
	{
		[Key]
		[Column(Order = 1)]
		[MaxLength(100)]
		[Required]
		[Index("symbol",Order =1)]
		public string Type { get; set; }

		[Key]
		[Column(Order = 2)]
		[MaxLength(100)]
		[Required]
		public string Category { get; set; }

		[Key]
		[Column(Order = 3)]
		[MaxLength(100)]
		[Required]
		[Index("symbol", Order = 2)]
		public string Symbol { get; set; }

		[ConcurrencyCheck]
		[Timestamp]
		[Display(Name = "乐观锁时间戳")]
		public byte[] TimeStamp { get; set; }

	}
}
