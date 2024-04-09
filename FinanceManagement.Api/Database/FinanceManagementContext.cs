using FinanceManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Api.Database;

public class FinanceManagementContext : DbContext
{
    public FinanceManagementContext(DbContextOptions<FinanceManagementContext> options) : base(options) { }

    public DbSet<IncomeComplete> Incomes { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceManagementContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
