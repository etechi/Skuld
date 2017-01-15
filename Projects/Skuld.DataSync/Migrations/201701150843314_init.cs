namespace Skuld.DataSync.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Type = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.Type, t.Name });
            
            CreateTable(
                "dbo.CategorySymbols",
                c => new
                    {
                        Type = c.String(nullable: false, maxLength: 100),
                        Category = c.String(nullable: false, maxLength: 100),
                        Symbol = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => new { t.Type, t.Category, t.Symbol });
            
            CreateTable(
                "dbo.CategoryTypes",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Prices",
                c => new
                    {
                        Symbol = c.String(nullable: false, maxLength: 20),
                        Interval = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Open = c.Single(nullable: false),
                        Close = c.Single(nullable: false),
                        High = c.Single(nullable: false),
                        Low = c.Single(nullable: false),
                        Volume = c.Single(nullable: false),
                        AdjustRate = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.Symbol, t.Interval, t.Time });
            
            CreateTable(
                "dbo.SymbolPropertyCategories",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.SymbolPropertyItems",
                c => new
                    {
                        Category = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => new { t.Category, t.Name });
            
            CreateTable(
                "dbo.SymbolPropertyUpdates",
                c => new
                    {
                        Symbol = c.String(nullable: false, maxLength: 20),
                        Category = c.String(nullable: false, maxLength: 50),
                        Time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.Symbol, t.Category, t.Time });
            
            CreateTable(
                "dbo.SymbolPropertyValues",
                c => new
                    {
                        Symbol = c.String(nullable: false, maxLength: 20),
                        Category = c.String(nullable: false, maxLength: 50),
                        Time = c.DateTime(nullable: false),
                        Property = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => new { t.Symbol, t.Category, t.Time, t.Property });
            
            CreateTable(
                "dbo.Symbols",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Type = c.Int(nullable: false),
                        ScopeCode = c.String(nullable: false, maxLength: 20),
                        Code = c.String(nullable: false, maxLength: 100),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Symbols");
            DropTable("dbo.SymbolPropertyValues");
            DropTable("dbo.SymbolPropertyUpdates");
            DropTable("dbo.SymbolPropertyItems");
            DropTable("dbo.SymbolPropertyCategories");
            DropTable("dbo.Prices");
            DropTable("dbo.CategoryTypes");
            DropTable("dbo.CategorySymbols");
            DropTable("dbo.Categories");
        }
    }
}
