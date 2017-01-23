using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Skuld.DataSync;
using Skuld;

namespace Skuld.DataSync.Migrations
{
    [DbContext(typeof(AppContext))]
    [Migration("20170123113919_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.Category", b =>
                {
                    b.Property<string>("Type")
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Type", "Name");

                    b.ToTable("SkuldCategories");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.CategorySymbol", b =>
                {
                    b.Property<string>("Type")
                        .HasMaxLength(100);

                    b.Property<string>("Category")
                        .HasMaxLength(100);

                    b.Property<string>("Symbol")
                        .HasMaxLength(100);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Type", "Category", "Symbol");

                    b.HasIndex("Type", "Symbol");

                    b.ToTable("SkuldCategorySymbols");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.CategoryType", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Name");

                    b.ToTable("SkuldCategoryTypes");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.Price", b =>
                {
                    b.Property<string>("Symbol")
                        .HasMaxLength(20);

                    b.Property<int>("Interval");

                    b.Property<DateTime>("Time");

                    b.Property<float>("AdjustRate");

                    b.Property<float>("Close");

                    b.Property<float>("High");

                    b.Property<float>("Low");

                    b.Property<float>("Open");

                    b.Property<float>("Volume");

                    b.HasKey("Symbol", "Interval", "Time");

                    b.ToTable("SkuldPrices");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.PropertyGroup", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Name");

                    b.ToTable("SkuldPropertyGroups");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.PropertyItem", b =>
                {
                    b.Property<string>("Group")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.Property<byte[]>("TimeStamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("Group", "Name");

                    b.ToTable("SkuldPropertyItems");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.Symbol", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("ScopeCode")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("SkuldSymbols");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.SymbolPropertyGroup", b =>
                {
                    b.Property<string>("Symbol")
                        .HasMaxLength(20);

                    b.Property<string>("Group")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("NextUpdateTime");

                    b.Property<int>("RowCount");

                    b.Property<DateTime>("Time");

                    b.HasKey("Symbol", "Group");

                    b.ToTable("SkuldSymbolPropertyGroups");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.SymbolPropertyGroupHistory", b =>
                {
                    b.Property<string>("Symbol")
                        .HasMaxLength(20);

                    b.Property<string>("Group")
                        .HasMaxLength(50);

                    b.Property<DateTime>("Time");

                    b.Property<int>("RowCount");

                    b.HasKey("Symbol", "Group", "Time");

                    b.ToTable("SkuldSymbolPropertyGroupHistories");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.SymbolPropertyValue", b =>
                {
                    b.Property<string>("Symbol")
                        .HasMaxLength(20);

                    b.Property<string>("Group")
                        .HasMaxLength(50);

                    b.Property<string>("Property")
                        .HasMaxLength(50);

                    b.Property<int>("Row");

                    b.Property<double?>("Number");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.HasKey("Symbol", "Group", "Property", "Row");

                    b.ToTable("SkuldSymbolPropertyValues");
                });

            modelBuilder.Entity("Skuld.DataStorages.Entity.Models.SymbolPropertyValueHistory", b =>
                {
                    b.Property<string>("Symbol")
                        .HasMaxLength(20);

                    b.Property<string>("Group")
                        .HasMaxLength(50);

                    b.Property<DateTime>("Time");

                    b.Property<string>("Property")
                        .HasMaxLength(50);

                    b.Property<int>("Row");

                    b.Property<double?>("Number");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.HasKey("Symbol", "Group", "Time", "Property", "Row");

                    b.ToTable("SkuldSymbolPropertyValueHistories");
                });
        }
    }
}
