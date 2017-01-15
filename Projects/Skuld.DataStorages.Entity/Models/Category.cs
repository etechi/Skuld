using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Skuld.DataStorage.EFCore.Models
{
	public class Category
	{
		[Key]
		[Column(Order =1)]
		[MaxLength(100)]
		[Required]
		public string Type { get; set; }

		[Key]
		[Column(Order =2)]
		[MaxLength(100)]
		[Required]
		public string Name { get; set; }
    }
}
