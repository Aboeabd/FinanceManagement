using FinanceManagement.Api.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Api.Database
{
    public class ExpenseConfig : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.HasKey(entity => entity.Id);

            builder.HasIndex(entity => entity.OneTimeExpenseDate);

            builder.Property(entity => entity.Id)
                .HasColumnName("ExpenseId");

            builder.Property(entity => entity.ExpenseType)
                .HasColumnName("ExpenseType")
                .IsRequired();

            builder.Property(entity => entity.Amount)
                .HasColumnType("decimal(15, 2)")
                .IsRequired();

            builder.Property(entity => entity.OneTimeExpenseDate)
                .HasColumnName("OneTimeExpenseDate")
                .HasColumnType("DATETIME2(3)");
        }

    }
}