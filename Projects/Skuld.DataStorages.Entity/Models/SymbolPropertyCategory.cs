using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorage.EFCore.Models
{
	public class SymbolPropertyCategory
	{
		[Key]
		[MaxLength(50)]
		[Required]
		public string Name { get; set; }
    }
}
