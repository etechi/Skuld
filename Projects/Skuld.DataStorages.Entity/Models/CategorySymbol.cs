using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorage.EFCore.Models
{
	public class CategorySymbol
	{
		[Key]
		[Column(Order = 1)]
		[MaxLength(100)]
		[Required]
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
		public string Symbol { get; set; }

	}
}
