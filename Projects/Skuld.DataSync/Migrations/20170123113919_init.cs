using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Skuld.DataSync.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SkuldCategories",
                columns: table => new
                {
                    Type = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldCategories", x => new { x.Type, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "SkuldCategorySymbols",
                columns: table => new
                {
                    Type = table.Column<string>(maxLength: 100, nullable: false),
                    Category = table.Column<string>(maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(maxLength: 100, nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldCategorySymbols", x => new { x.Type, x.Category, x.Symbol });
                });

            migrationBuilder.CreateTable(
                name: "SkuldCategoryTypes",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldCategoryTypes", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "SkuldPrices",
                columns: table => new
                {
                    Symbol = table.Column<string>(maxLength: 20, nullable: false),
                    Interval = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    AdjustRate = table.Column<float>(nullable: false),
                    Close = table.Column<float>(nullable: false),
                    High = table.Column<float>(nullable: false),
                    Low = table.Column<float>(nullable: false),
                    Open = table.Column<float>(nullable: false),
                    Volume = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldPrices", x => new { x.Symbol, x.Interval, x.Time });
                });

            migrationBuilder.CreateTable(
                name: "SkuldPropertyGroups",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldPropertyGroups", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "SkuldPropertyItems",
                columns: table => new
                {
                    Group = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldPropertyItems", x => new { x.Group, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "SkuldSymbols",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Code = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ScopeCode = table.Column<string>(maxLength: 20, nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldSymbols", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkuldSymbolPropertyGroups",
                columns: table => new
                {
                    Symbol = table.Column<string>(maxLength: 20, nullable: false),
                    Group = table.Column<string>(maxLength: 50, nullable: false),
                    NextUpdateTime = table.Column<DateTime>(nullable: true),
                    RowCount = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldSymbolPropertyGroups", x => new { x.Symbol, x.Group });
                });

            migrationBuilder.CreateTable(
                name: "SkuldSymbolPropertyGroupHistories",
                columns: table => new
                {
                    Symbol = table.Column<string>(maxLength: 20, nullable: false),
                    Group = table.Column<string>(maxLength: 50, nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    RowCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldSymbolPropertyGroupHistories", x => new { x.Symbol, x.Group, x.Time });
                });

            migrationBuilder.CreateTable(
                name: "SkuldSymbolPropertyValues",
                columns: table => new
                {
                    Symbol = table.Column<string>(maxLength: 20, nullable: false),
                    Group = table.Column<string>(maxLength: 50, nullable: false),
                    Property = table.Column<string>(maxLength: 50, nullable: false),
                    Row = table.Column<int>(nullable: false),
                    Number = table.Column<double>(nullable: true),
                    Value = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldSymbolPropertyValues", x => new { x.Symbol, x.Group, x.Property, x.Row });
                });

            migrationBuilder.CreateTable(
                name: "SkuldSymbolPropertyValueHistories",
                columns: table => new
                {
                    Symbol = table.Column<string>(maxLength: 20, nullable: false),
                    Group = table.Column<string>(maxLength: 50, nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Property = table.Column<string>(maxLength: 50, nullable: false),
                    Row = table.Column<int>(nullable: false),
                    Number = table.Column<double>(nullable: true),
                    Value = table.Column<string>(maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkuldSymbolPropertyValueHistories", x => new { x.Symbol, x.Group, x.Time, x.Property, x.Row });
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkuldCategorySymbols_Type_Symbol",
                table: "SkuldCategorySymbols",
                columns: new[] { "Type", "Symbol" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkuldCategories");

            migrationBuilder.DropTable(
                name: "SkuldCategorySymbols");

            migrationBuilder.DropTable(
                name: "SkuldCategoryTypes");

            migrationBuilder.DropTable(
                name: "SkuldPrices");

            migrationBuilder.DropTable(
                name: "SkuldPropertyGroups");

            migrationBuilder.DropTable(
                name: "SkuldPropertyItems");

            migrationBuilder.DropTable(
                name: "SkuldSymbols");

            migrationBuilder.DropTable(
                name: "SkuldSymbolPropertyGroups");

            migrationBuilder.DropTable(
                name: "SkuldSymbolPropertyGroupHistories");

            migrationBuilder.DropTable(
                name: "SkuldSymbolPropertyValues");

            migrationBuilder.DropTable(
                name: "SkuldSymbolPropertyValueHistories");
        }
    }
}
