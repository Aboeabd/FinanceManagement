using System;
using System.Linq;
using FinanceManagement.Api.Database;
using FinanceManagement.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FinanceManagement.Tests.Database
{
    public class FinanceManagementContextShould
    {
        [Fact]
        public void CanAddAndRetrieveIncome()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<FinanceManagementContext>()
                .UseInMemoryDatabase(databaseName: "CanAddAndRetrieveIncome")
                .Options;

            var income = new IncomeComplete { IncomeType = IncomeType.Salary, Amount = 100, OneTimeIncomeDate = DateTime.Now };

            // Act
            using (var context = new FinanceManagementContext(options))
            {
                context.Incomes.Add(income);
                context.SaveChanges();
            }

            // Assert
            using (var context = new FinanceManagementContext(options))
            {
                var retrievedIncome = context.Incomes.Single();
                Assert.Equal(income.IncomeType, retrievedIncome.IncomeType);
                Assert.Equal(income.Amount, retrievedIncome.Amount);
            }
        }
    }
}