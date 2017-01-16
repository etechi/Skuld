using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorages.Entity.Models
{
	[Table("PropertyGroups")]
	public class PropertyGroup
	{
		[Key]
		[MaxLength(50)]
		[Required]
		public string Name { get; set; }
		[ConcurrencyCheck]
		[Timestamp]
		[Display(Name = "乐观锁时间戳")]
		public byte[] TimeStamp { get; set; }

	}
}
