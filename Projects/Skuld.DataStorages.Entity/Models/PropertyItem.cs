using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorages.Entity.Models
{
	[Table("PropertyItems")]
	public class PropertyItem
	{
		[Key]
		[Column(Order = 1)]
		[MaxLength(50)]
		[Required]
		public string Group { get; set; }

		[Key]
		[Column(Order = 2)]
		[MaxLength(50)]
		[Required]
		public string Name { get; set; }

		[ConcurrencyCheck]
		[Timestamp]
		[Display(Name = "乐观锁时间戳")]
		public byte[] TimeStamp { get; set; }

	}
}
