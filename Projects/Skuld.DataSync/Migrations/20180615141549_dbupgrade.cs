using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Skuld.DataSync.Migrations
{
    public partial class dbupgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysCallExpired");

            migrationBuilder.DropTable(
                name: "SysCallInstance");

            migrationBuilder.DropIndex(
                name: "IX_SysServiceInstance_ImplementType",
                table: "SysServiceInstance");

            migrationBuilder.DropIndex(
                name: "IX_SysServiceInstance_ContainerId_ServiceType",
                table: "SysServiceInstance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Symbols",
                table: "Symbols");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SymbolPropertyValues",
                table: "SymbolPropertyValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SymbolPropertyValueHistories",
                table: "SymbolPropertyValueHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SymbolPropertyGroups",
                table: "SymbolPropertyGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SymbolPropertyGroupHistories",
                table: "SymbolPropertyGroupHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyItems",
                table: "PropertyItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyGroups",
                table: "PropertyGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prices",
                table: "Prices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryTypes",
                table: "CategoryTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySymbols",
                table: "CategorySymbols");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Symbols",
                newName: "SkuldSymbols");

            migrationBuilder.RenameTable(
                name: "SymbolPropertyValues",
                newName: "SkuldSymbolPropertyValues");

            migrationBuilder.RenameTable(
                name: "SymbolPropertyValueHistories",
                newName: "SkuldSymbolPropertyValueHistories");

            migrationBuilder.RenameTable(
                name: "SymbolPropertyGroups",
                newName: "SkuldSymbolPropertyGroups");

            migrationBuilder.RenameTable(
                name: "SymbolPropertyGroupHistories",
                newName: "SkuldSymbolPropertyGroupHistories");

            migrationBuilder.RenameTable(
                name: "PropertyItems",
                newName: "SkuldPropertyItems");

            migrationBuilder.RenameTable(
                name: "PropertyGroups",
                newName: "SkuldPropertyGroups");

            migrationBuilder.RenameTable(
                name: "Prices",
                newName: "SkuldPrices");

            migrationBuilder.RenameTable(
                name: "CategoryTypes",
                newName: "SkuldCategoryTypes");

            migrationBuilder.RenameTable(
                name: "CategorySymbols",
                newName: "SkuldCategorySymbols");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "SkuldCategories");

            migrationBuilder.RenameColumn(
                name: "ScopeId",
                table: "SysServiceInstance",
                newName: "ServiceDataScopeId");

            migrationBuilder.RenameIndex(
                name: "IX_SysServiceInstance_ScopeId",
                table: "SysServiceInstance",
                newName: "IX_SysServiceInstance_ServiceDataScopeId");

            migrationBuilder.RenameIndex(
                name: "IX_CategorySymbols_Type_Symbol",
                table: "SkuldCategorySymbols",
                newName: "IX_SkuldCategorySymbols_Type_Symbol");

            migrationBuilder.AlterColumn<long>(
                name: "UpdatorId",
                table: "SysServiceInstance",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "ServiceType",
                table: "SysServiceInstance",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                table: "SysServiceInstance",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "ImplementType",
                table: "SysServiceInstance",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 300);

            migrationBuilder.AddColumn<string>(
                name: "ImplementId",
                table: "SysServiceInstance",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InternalRemarks",
                table: "SysServiceInstance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceId",
                table: "SysServiceInstance",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "SysIdentSeed",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldSymbols",
                table: "SkuldSymbols",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldSymbolPropertyValues",
                table: "SkuldSymbolPropertyValues",
                columns: new[] { "Symbol", "Group", "Property", "Row" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldSymbolPropertyValueHistories",
                table: "SkuldSymbolPropertyValueHistories",
                columns: new[] { "Symbol", "Group", "Time", "Property", "Row" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldSymbolPropertyGroups",
                table: "SkuldSymbolPropertyGroups",
                columns: new[] { "Symbol", "Group" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldSymbolPropertyGroupHistories",
                table: "SkuldSymbolPropertyGroupHistories",
                columns: new[] { "Symbol", "Group", "Time" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldPropertyItems",
                table: "SkuldPropertyItems",
                columns: new[] { "Group", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldPropertyGroups",
                table: "SkuldPropertyGroups",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldPrices",
                table: "SkuldPrices",
                columns: new[] { "Symbol", "Interval", "Time" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldCategoryTypes",
                table: "SkuldCategoryTypes",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldCategorySymbols",
                table: "SkuldCategorySymbols",
                columns: new[] { "Type", "Category", "Symbol" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SkuldCategories",
                table: "SkuldCategories",
                columns: new[] { "Type", "Name" });

            migrationBuilder.CreateTable(
                name: "SysReminder",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TaskState = table.Column<int>(nullable: false),
                    TaskExecCount = table.Column<int>(nullable: false),
                    TaskMessage = table.Column<string>(nullable: true),
                    TaskStartTime = table.Column<DateTime>(nullable: true),
                    TaskLastExecTime = table.Column<DateTime>(nullable: true),
                    TaskNextExecTime = table.Column<DateTime>(nullable: true),
                    BizType = table.Column<string>(nullable: true),
                    BizIdentType = table.Column<string>(nullable: true),
                    BizIdent = table.Column<long>(nullable: false),
                    Data = table.Column<string>(maxLength: 1000, nullable: true),
                    RemindableName = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysReminder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysRemindRecord",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    ServiceDataScopeId = table.Column<long>(nullable: true),
                    LogicState = table.Column<byte>(nullable: false),
                    OwnerId = table.Column<long>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    UpdatorId = table.Column<long>(nullable: true),
                    UpdatedTime = table.Column<DateTime>(nullable: false),
                    InternalRemarks = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TaskState = table.Column<int>(nullable: false),
                    TaskExecCount = table.Column<int>(nullable: false),
                    TaskMessage = table.Column<string>(nullable: true),
                    TaskStartTime = table.Column<DateTime>(nullable: true),
                    TaskLastExecTime = table.Column<DateTime>(nullable: true),
                    TaskNextExecTime = table.Column<DateTime>(nullable: true),
                    BizType = table.Column<string>(nullable: true),
                    BizIdentType = table.Column<string>(nullable: true),
                    BizIdent = table.Column<long>(nullable: false),
                    Data = table.Column<string>(maxLength: 1000, nullable: true),
                    RemindableName = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysRemindRecord", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ImplementId",
                table: "SysServiceInstance",
                column: "ImplementId");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ContainerId_ServiceId",
                table: "SysServiceInstance",
                columns: new[] { "ContainerId", "ServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_CreatedTime",
                table: "SysReminder",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_Name",
                table: "SysReminder",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_OwnerId",
                table: "SysReminder",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_ServiceDataScopeId",
                table: "SysReminder",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_UpdatorId",
                table: "SysReminder",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_TaskState_TaskNextExecTime",
                table: "SysReminder",
                columns: new[] { "TaskState", "TaskNextExecTime" });

            migrationBuilder.CreateIndex(
                name: "IX_SysReminder_BizType_BizIdentType_BizIdent",
                table: "SysReminder",
                columns: new[] { "BizType", "BizIdentType", "BizIdent" },
                unique: true,
                filter: "[BizType] IS NOT NULL AND [BizIdentType] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_CreatedTime",
                table: "SysRemindRecord",
                column: "CreatedTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_Name",
                table: "SysRemindRecord",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_OwnerId",
                table: "SysRemindRecord",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_ServiceDataScopeId",
                table: "SysRemindRecord",
                column: "ServiceDataScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_UpdatorId",
                table: "SysRemindRecord",
                column: "UpdatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_TaskState_TaskNextExecTime",
                table: "SysRemindRecord",
                columns: new[] { "TaskState", "TaskNextExecTime" });

            migrationBuilder.CreateIndex(
                name: "IX_SysRemindRecord_BizType_BizIdentType_BizIdent",
                table: "SysRemindRecord",
                columns: new[] { "BizType", "BizIdentType", "BizIdent" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysReminder");

            migrationBuilder.DropTable(
                name: "SysRemindRecord");

            migrationBuilder.DropIndex(
                name: "IX_SysServiceInstance_ImplementId",
                table: "SysServiceInstance");

            migrationBuilder.DropIndex(
                name: "IX_SysServiceInstance_ContainerId_ServiceId",
                table: "SysServiceInstance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldSymbols",
                table: "SkuldSymbols");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldSymbolPropertyValues",
                table: "SkuldSymbolPropertyValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldSymbolPropertyValueHistories",
                table: "SkuldSymbolPropertyValueHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldSymbolPropertyGroups",
                table: "SkuldSymbolPropertyGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldSymbolPropertyGroupHistories",
                table: "SkuldSymbolPropertyGroupHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldPropertyItems",
                table: "SkuldPropertyItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldPropertyGroups",
                table: "SkuldPropertyGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldPrices",
                table: "SkuldPrices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldCategoryTypes",
                table: "SkuldCategoryTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldCategorySymbols",
                table: "SkuldCategorySymbols");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SkuldCategories",
                table: "SkuldCategories");

            migrationBuilder.DropColumn(
                name: "ImplementId",
                table: "SysServiceInstance");

            migrationBuilder.DropColumn(
                name: "InternalRemarks",
                table: "SysServiceInstance");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "SysServiceInstance");

            migrationBuilder.RenameTable(
                name: "SkuldSymbols",
                newName: "Symbols");

            migrationBuilder.RenameTable(
                name: "SkuldSymbolPropertyValues",
                newName: "SymbolPropertyValues");

            migrationBuilder.RenameTable(
                name: "SkuldSymbolPropertyValueHistories",
                newName: "SymbolPropertyValueHistories");

            migrationBuilder.RenameTable(
                name: "SkuldSymbolPropertyGroups",
                newName: "SymbolPropertyGroups");

            migrationBuilder.RenameTable(
                name: "SkuldSymbolPropertyGroupHistories",
                newName: "SymbolPropertyGroupHistories");

            migrationBuilder.RenameTable(
                name: "SkuldPropertyItems",
                newName: "PropertyItems");

            migrationBuilder.RenameTable(
                name: "SkuldPropertyGroups",
                newName: "PropertyGroups");

            migrationBuilder.RenameTable(
                name: "SkuldPrices",
                newName: "Prices");

            migrationBuilder.RenameTable(
                name: "SkuldCategoryTypes",
                newName: "CategoryTypes");

            migrationBuilder.RenameTable(
                name: "SkuldCategorySymbols",
                newName: "CategorySymbols");

            migrationBuilder.RenameTable(
                name: "SkuldCategories",
                newName: "Categories");

            migrationBuilder.RenameColumn(
                name: "ServiceDataScopeId",
                table: "SysServiceInstance",
                newName: "ScopeId");

            migrationBuilder.RenameIndex(
                name: "IX_SysServiceInstance_ServiceDataScopeId",
                table: "SysServiceInstance",
                newName: "IX_SysServiceInstance_ScopeId");

            migrationBuilder.RenameIndex(
                name: "IX_SkuldCategorySymbols_Type_Symbol",
                table: "CategorySymbols",
                newName: "IX_CategorySymbols_Type_Symbol");

            migrationBuilder.AlterColumn<long>(
                name: "UpdatorId",
                table: "SysServiceInstance",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceType",
                table: "SysServiceInstance",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                table: "SysServiceInstance",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImplementType",
                table: "SysServiceInstance",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "SysIdentSeed",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Symbols",
                table: "Symbols",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SymbolPropertyValues",
                table: "SymbolPropertyValues",
                columns: new[] { "Symbol", "Group", "Property", "Row" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SymbolPropertyValueHistories",
                table: "SymbolPropertyValueHistories",
                columns: new[] { "Symbol", "Group", "Time", "Property", "Row" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SymbolPropertyGroups",
                table: "SymbolPropertyGroups",
                columns: new[] { "Symbol", "Group" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SymbolPropertyGroupHistories",
                table: "SymbolPropertyGroupHistories",
                columns: new[] { "Symbol", "Group", "Time" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyItems",
                table: "PropertyItems",
                columns: new[] { "Group", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyGroups",
                table: "PropertyGroups",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prices",
                table: "Prices",
                columns: new[] { "Symbol", "Interval", "Time" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryTypes",
                table: "CategoryTypes",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySymbols",
                table: "CategorySymbols",
                columns: new[] { "Type", "Category", "Symbol" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                columns: new[] { "Type", "Name" });

            migrationBuilder.CreateTable(
                name: "SysCallExpired",
                columns: table => new
                {
                    Callable = table.Column<string>(maxLength: 200, nullable: false),
                    CallArgument = table.Column<string>(maxLength: 200, nullable: true),
                    CallError = table.Column<string>(maxLength: 200, nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    ExecCount = table.Column<int>(nullable: false),
                    ExecError = table.Column<string>(maxLength: 200, nullable: true),
                    Expired = table.Column<DateTime>(nullable: false),
                    LastExecTime = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysCallExpired", x => x.Callable);
                });

            migrationBuilder.CreateTable(
                name: "SysCallInstance",
                columns: table => new
                {
                    Callable = table.Column<string>(maxLength: 200, nullable: false),
                    CallArgument = table.Column<string>(maxLength: 200, nullable: true),
                    CallError = table.Column<string>(maxLength: 200, nullable: true),
                    CallTime = table.Column<DateTime>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    DelaySecondsOnError = table.Column<int>(nullable: false),
                    ErrorCount = table.Column<int>(nullable: false),
                    ExecError = table.Column<string>(maxLength: 200, nullable: true),
                    Expire = table.Column<DateTime>(nullable: false),
                    LastExecTime = table.Column<DateTime>(nullable: true),
                    TimeStamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysCallInstance", x => x.Callable);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ImplementType",
                table: "SysServiceInstance",
                column: "ImplementType");

            migrationBuilder.CreateIndex(
                name: "IX_SysServiceInstance_ContainerId_ServiceType",
                table: "SysServiceInstance",
                columns: new[] { "ContainerId", "ServiceType" });

            migrationBuilder.CreateIndex(
                name: "IX_SysCallInstance_CallTime",
                table: "SysCallInstance",
                column: "CallTime");
        }
    }
}
