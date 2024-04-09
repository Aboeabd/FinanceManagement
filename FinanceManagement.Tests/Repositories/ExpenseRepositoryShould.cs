using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceManagement.Api.Controllers.Interfaces;
using FinanceManagement.Api.Database;
using FinanceManagement.Api.Domain;
using FinanceManagement.Api.Domain.Dto;
using FinanceManagement.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace FinanceManagement.Tests.Repositories
{
    [Collection("Sequential")]
    public class ExpenseRepositoryTests
    {
        private readonly FinanceManagementContext _context;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<FinanceManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new FinanceManagementContext(options);
            _context.Database.EnsureDeleted(); // This will delete the database before each test
            _context.Database.EnsureCreated(); // This will create a new database before each test

            _cacheMock = new Mock<IMemoryCache>();
            _expenseRepository = new ExpenseRepository(_context, _cacheMock.Object);
        }

        [Fact]
        public async Task GetExpenses_ReturnsExpenses_WhenExpensesExist()
        {
            // Arrange
            var expenses = new List<Expense>
            {
                new Expense
                {
                    Id = Guid.NewGuid(),
                    ExpenseType = "Rent",
                    Amount = 100,
                    OneTimeExpenseDate = DateTime.Now
                },
                new Expense
                {
                    Id = Guid.NewGuid(),
                    ExpenseType = "Groceries",
                    Amount = 200,
                    OneTimeExpenseDate = DateTime.Now
                }
            };

            var cacheEntryMock = new Mock<ICacheEntry>();

            _cacheMock.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

            cacheEntryMock
                .SetupSet(m => m.Value = It.IsAny<List<ExpenseDto>>())
                .Callback<object>(value => cacheEntryMock.Setup(m => m.Value).Returns(value));

            _context.Expenses.AddRange(expenses);
            await _context.SaveChangesAsync();

            // Act
            var result = await _expenseRepository.GetExpenses();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expenses.Count, result.Count());
        }

        [Fact]
        public async Task GetExpenseById_ReturnsExpense_WhenExpenseExists()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var expense = new Expense
            {
                Id = expenseId,
                ExpenseType = "Rent",
                Amount = 100,
                OneTimeExpenseDate = DateTime.Now
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            // Act
            var result = await _expenseRepository.GetExpenseById(expenseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expenseId, result.Id);
        }

        [Fact]
        public async Task AddNewExpense_ShouldAddExpense_WhenExpenseIsValid()
        {
            // Arrange
            var expense = new Expense
            {
                ExpenseType = "Groceries",
                Amount = 200,
                OneTimeExpenseDate = DateTime.Now
            };

            // Act
            var result = await _expenseRepository.AddNewExpense(expense);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expense.Amount, result.Amount);
            Assert.Equal(expense.ExpenseType, result.ExpenseType);
        }

        [Fact]
        public async Task UpdateExpense_ShouldUpdateExpense_WhenExpenseExists()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var expenseToUpdate = new Expense
            {
                Id = expenseId,
                ExpenseType = "Groceries",
                Amount = 200,
                OneTimeExpenseDate = DateTime.Now
            };

            _context.Expenses.Add(expenseToUpdate);
            await _context.SaveChangesAsync();

            var updatedExpense = new Expense
            {
                Id = expenseId,
                ExpenseType = "Rent",
                Amount = 100,
                OneTimeExpenseDate = DateTime.Now
            };

            // Act
            var result = await _expenseRepository.UpdateExpense(expenseId, updatedExpense);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedExpense.ExpenseType, result.ExpenseType);
        }

        [Fact]
        public async Task DeleteExpense_ShouldReturnTrue_WhenExpenseIsDeleted()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var expenseToDelete = new Expense
            {
                Id = expenseId,
                ExpenseType = "Groceries",
                Amount = 200,
                OneTimeExpenseDate = DateTime.Now
            };

            _context.Expenses.Add(expenseToDelete);
            await _context.SaveChangesAsync();

            // Act
            var result = await _expenseRepository.DeleteExpense(expenseId);

            // Assert
            Assert.True(result);
        }
    }
}
