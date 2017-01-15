﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorage.EFCore.Models
{
	public class SymbolPropertyUpdate
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
		public string Category { get; set; }

		[Key]
		[Column(Order = 3)]
		public DateTime Time { get; set; }

	}
}
