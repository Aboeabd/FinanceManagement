using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManagement.Api.Controllers.Interfaces;
using FinanceManagement.Api.Database;
using FinanceManagement.Api.Domain;
using FinanceManagement.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace FinanceManagement.Tests.Repositories
{
    [Collection("Sequential")]
    public class IncomeRepositoryShould
    {
        private readonly FinanceManagementContext _context;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly IIncomeRepository _incomeRepository;

        public IncomeRepositoryShould()
        {
            var options = new DbContextOptionsBuilder<FinanceManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new FinanceManagementContext(options);
            _context.Database.EnsureDeleted(); // This will delete the database before each test
            _context.Database.EnsureCreated(); // This will create a new database before each test

            _cacheMock = new Mock<IMemoryCache>();
            _incomeRepository = new IncomeRepository(_context, _cacheMock.Object);
        }

        [Fact]
        public async Task GetAllIncome_ReturnsIncomes_WhenIncomesExist()
        {
            // Arrange
            var incomes = new List<IncomeComplete>
            {
                new IncomeComplete
                {
                    Id = Guid.NewGuid(),
                    Amount = 100,
                    IncomeType = IncomeType.Salary,
                    OneTimeIncomeDate = DateTime.Now
                },
                new IncomeComplete
                {
                    Id = Guid.NewGuid(),
                    Amount = 200,
                    IncomeType = IncomeType.Freelance,
                    OneTimeIncomeDate = DateTime.Now
                }
            };

            _context.Incomes.AddRange(incomes);
            await _context.SaveChangesAsync();

            var cacheEntryMock = new Mock<ICacheEntry>();

            _cacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

            cacheEntryMock
                .SetupSet(m => m.Value = It.IsAny<List<IncomeComplete>>())
                .Callback<object>(value => cacheEntryMock.Setup(m => m.Value).Returns(value));

            // Act
            var result = await _incomeRepository.GetAllIncome();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(incomes.Count, result.Count());
        }

        [Fact]
        public async Task GetIncomeById_ReturnsIncome_WhenIncomeExists()
        {
            // Arrange
            var incomeId = Guid.NewGuid();
            var income = new IncomeComplete
            {
                Id = incomeId,
                Amount = 100,
                IncomeType = IncomeType.Salary,
                OneTimeIncomeDate = DateTime.Now
            };

            _context.Incomes.Add(income);
            await _context.SaveChangesAsync();

            var cacheEntryMock = new Mock<ICacheEntry>();

            _cacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

            cacheEntryMock
                .SetupSet(m => m.Value = It.IsAny<List<IncomeComplete>>())
                .Callback<object>(value => cacheEntryMock.Setup(m => m.Value).Returns(value));

            // Act
            var result = await _incomeRepository.GetIncomeById(incomeId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(incomeId, result.Id);
        }

        [Fact]
        public async Task AddNewIncome_ShouldAddIncome_WhenIncomeIsValid()
        {
            // Arrange
            var income = new Income
            {
                Amount = 100,
                IncomeType = IncomeType.Salary,
                OneTimeIncomeDate = DateTime.Now
            };

            // Act
            var result = await _incomeRepository.AddNewIncome(income);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(income.Amount, result.Amount);
            Assert.Equal(income.IncomeType, result.IncomeType);
        }

        [Fact]
        public async Task UpdateIncome_ShouldUpdateIncome_WhenIncomeExists()
        {
            // Arrange
            var incomeId = Guid.NewGuid();
            var incomeToUpdate = new IncomeComplete
            {
                Id = incomeId,
                Amount = 100,
                IncomeType = IncomeType.Salary,
                OneTimeIncomeDate = DateTime.Now
            };

            _context.Incomes.Add(incomeToUpdate);
            await _context.SaveChangesAsync();

            var updatedIncome = new Income
            {
                Amount = 200,
                IncomeType = IncomeType.Freelance,
                OneTimeIncomeDate = DateTime.Now
            };

            // Act
            var result = await _incomeRepository.UpdateIncome(incomeId, updatedIncome);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedIncome.Amount, result.Amount);
            Assert.Equal(updatedIncome.IncomeType, result.IncomeType);
        }

        [Fact]
        public async Task DeleteIncome_ShouldReturnTrue_WhenIncomeIsDeleted()
        {
            // Arrange
            var incomeId = Guid.NewGuid();
            var incomeToDelete = new IncomeComplete
            {
                Id = incomeId,
                Amount = 100,
                IncomeType = IncomeType.Salary,
                OneTimeIncomeDate = DateTime.Now
            };

            _context.Incomes.Add(incomeToDelete);
            await _context.SaveChangesAsync();

            // Act
            var result = await _incomeRepository.DeleteIncome(incomeId);

            // Assert
            Assert.True(result);
        }
    }
}
