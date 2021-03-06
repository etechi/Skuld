﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorages.Entity.Models
{
	[Table("SymbolPropertyValues")]
	public class SymbolPropertyValue
	{
		[Key]
		[Column(Order =1)]
		[MaxLength(20)]
		[Required]
		public string Symbol { get; set; }

		[Key]
		[Column(Order = 2)]
		[MaxLength(50)]
		[Required]
		public string Group { get; set; }

		[Key]
		[Column(Order = 3)]
		[MaxLength(50)]
		[Required]
		public string Property { get; set; }
		[Key]
		[Column(Order = 4)]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Row { get; set; }


		[Required]
		[MaxLength(1000)]
		public string Value { get; set; }

		public double? Number { get; set; }


	}
}
