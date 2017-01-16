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
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.Type, t.Name });
            
            CreateTable(
                "dbo.CategorySymbols",
                c => new
                    {
                        Type = c.String(nullable: false, maxLength: 100),
                        Category = c.String(nullable: false, maxLength: 100),
                        Symbol = c.String(nullable: false, maxLength: 100),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.Type, t.Category, t.Symbol })
                .Index(t => new { t.Type, t.Symbol }, name: "symbol");
            
            CreateTable(
                "dbo.CategoryTypes",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 50),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
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
                "dbo.PropertyGroups",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 50),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.PropertyItems",
                c => new
                    {
                        Group = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 50),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => new { t.Group, t.Name });
            
            CreateTable(
                "dbo.SymbolPropertyGroupHistories",
                c => new
                    {
                        Symbol = c.String(nullable: false, maxLength: 20),
                        Group = c.String(nullable: false, maxLength: 50),
                        Time = c.DateTime(nullable: false),
                        RowCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Symbol, t.Group, t.Time });
            
            CreateTable(
                "dbo.SymbolPropertyGroups",
                c => new
                    {
                        Symbol = c.String(nullable: false, maxLength: 20),
                        Group = c.String(nullable: false, maxLength: 50),
                        Time = c.DateTime(nullable: false),
                        RowCount = c.Int(nullable: false),
                        NextUpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.Symbol, t.Group });
            
            CreateTable(
                "dbo.SymbolPropertyValueHistories",
                c => new
                    {
                        Symbol = c.String(nullable: false, maxLength: 20),
                        Group = c.String(nullable: false, maxLength: 50),
                        Time = c.DateTime(nullable: false),
                        Property = c.String(nullable: false, maxLength: 50),
                        Row = c.Int(nullable: false),
                        Value = c.String(nullable: false, maxLength: 1000),
                        Number = c.Double(),
                    })
                .PrimaryKey(t => new { t.Symbol, t.Group, t.Time, t.Property, t.Row });
            
            CreateTable(
                "dbo.SymbolPropertyValues",
                c => new
                    {
                        Symbol = c.String(nullable: false, maxLength: 20),
                        Group = c.String(nullable: false, maxLength: 50),
                        Property = c.String(nullable: false, maxLength: 50),
                        Row = c.Int(nullable: false),
                        Value = c.String(nullable: false, maxLength: 1000),
                        Number = c.Double(),
                    })
                .PrimaryKey(t => new { t.Symbol, t.Group, t.Property, t.Row });
            
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
            DropIndex("dbo.CategorySymbols", "symbol");
            DropTable("dbo.Symbols");
            DropTable("dbo.SymbolPropertyValues");
            DropTable("dbo.SymbolPropertyValueHistories");
            DropTable("dbo.SymbolPropertyGroups");
            DropTable("dbo.SymbolPropertyGroupHistories");
            DropTable("dbo.PropertyItems");
            DropTable("dbo.PropertyGroups");
            DropTable("dbo.Prices");
            DropTable("dbo.CategoryTypes");
            DropTable("dbo.CategorySymbols");
            DropTable("dbo.Categories");
        }
    }
}
