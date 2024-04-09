using FinanceManagement.Api.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Api.Database;

public class IncomeCompleteConfig : IEntityTypeConfiguration<IncomeComplete>
{
    public void Configure(EntityTypeBuilder<IncomeComplete> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.HasIndex(entity => entity.OneTimeIncomeDate);

        builder.Property(entity => entity.Id)
            .HasColumnName("IncomeId");

        builder.Property(entity => entity.IncomeType)
            .HasColumnName("IncomeType")
            .IsRequired();

        builder.Property(entity => entity.Amount)
            .HasColumnType("decimal(15, 2)")
            .IsRequired();

        builder.Property(entity => entity.OneTimeIncomeDate)
            .HasColumnName("OneTimeIncomeDate")
            .HasColumnType("DATETIME2(3)");
    }
}
