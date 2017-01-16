using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace Skuld.DataStorages.Entity
{
    public class AppContext : DbContext
    {
		//public string ConnectionString { get; }
		public AppContext() : this("name=skuld") { }
		public AppContext(string Name):base(Name)
		{
			//this.ConnectionString = ConnectionString;
		}
		//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		//{
		//	optionsBuilder.UseSqlServer(ConnectionString);
		//	base.OnConfiguring(optionsBuilder);
		//}

		public DbSet<Models.Symbol> Symbols { get; set; }
		public DbSet<Models.Price> Prices { get; set; }

		public DbSet<Models.Category> Categories { get; set; }
		public DbSet<Models.CategorySymbol> CategorySymbols { get; set; }
		public DbSet<Models.CategoryType> CategoryTypes { get; set; }

		public DbSet<Models.PropertyGroup> PropertyGroups { get; set; }
		public DbSet<Models.PropertyItem> PropertyItems { get; set; }
		public DbSet<Models.SymbolPropertyGroup> SymbolPropertyGroups { get; set; }
		public DbSet<Models.SymbolPropertyValue> SymbolPropertyValues { get; set; }
		public DbSet<Models.SymbolPropertyGroupHistory> SymbolPropertyGroupHistory { get; set; }
		public DbSet<Models.SymbolPropertyValueHistory> SymbolPropertyValueHistory { get; set; }
	}
}
