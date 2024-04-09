﻿// <auto-generated />
using System;
using FinanceManagement.Api.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FinanceManagement.Api.Migrations
{
    [DbContext(typeof(FinanceManagementContext))]
    partial class FinanceManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.27")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("FinanceManagement.Api.Domain.Expense", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ExpenseId");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(15,2)");

                    b.Property<string>("ExpenseType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ExpenseType");

                    b.Property<DateTime?>("OneTimeExpenseDate")
                        .HasColumnType("DATETIME2(3)")
                        .HasColumnName("OneTimeExpenseDate");

                    b.HasKey("Id");

                    b.HasIndex("OneTimeExpenseDate");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("FinanceManagement.Api.Domain.IncomeComplete", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("IncomeId");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(15,2)");

                    b.Property<int>("IncomeType")
                        .HasColumnType("int")
                        .HasColumnName("IncomeType");

                    b.Property<DateTime?>("OneTimeIncomeDate")
                        .HasColumnType("DATETIME2(3)")
                        .HasColumnName("OneTimeIncomeDate");

                    b.HasKey("Id");

                    b.HasIndex("OneTimeIncomeDate");

                    b.ToTable("Incomes");
                });
#pragma warning restore 612, 618
        }
    }
}