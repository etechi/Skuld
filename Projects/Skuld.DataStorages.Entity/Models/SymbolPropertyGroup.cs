using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorages.Entity.Models
{
	[Table("SymbolPropertyGroups")]
	public class SymbolPropertyGroup
	{
		[Key]
		[Column(Order = 1)]
		[MaxLength(20)]
		[Required]
		public string Symbol { get; set; }

		[Key]
		[Column(Order = 2)]
		[MaxLength(50)]
		[Required]
		public string Group { get; set; }

		public DateTime Time { get; set; }

		public int RowCount { get; set; }

		public DateTime? NextUpdateTime { get; set; }
	}
}
