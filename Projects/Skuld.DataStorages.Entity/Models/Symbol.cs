using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorage.EFCore.Models
{
	public class Symbol
	{
		[Key]
		public string Id { get; set; }

		public SymbolType Type { get; set; }

		[MaxLength(20)]
		[Required]
		public string ScopeCode { get; set; }

		[MaxLength(100)]
		[Required]
		public string Code { get; set; }

		[MaxLength(100)]
		[Required]
		public string Name { get; set; }

    }
}
