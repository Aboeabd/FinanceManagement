using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManagement.Api.Controllers;
using FinanceManagement.Api.Domain;
using FinanceManagement.Api.Domain.Dto;
using FinanceManagement.Api.Controllers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FinanceManagement.Api.Tests
{
    public class ExpenseControllerShould
    {
        private readonly Mock<IExpenseRepository> _mockRepo;
        private readonly ExpensesController _controller;

        public ExpenseControllerShould()
        {
            _mockRepo = new Mock<IExpenseRepository>();
            _controller = new ExpensesController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetExpenses_ReturnsOkResult_WithListOfExpenses()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetExpenses()).ReturnsAsync(GetTestExpenses());

            // Act
            var result = await _controller.GetExpenses();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsType<List<ExpenseDto>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public async Task GetExpenseById_ReturnsNotFoundResult_WhenIdDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetExpenseById(It.IsAny<Guid>())).ReturnsAsync((Expense)null);

            // Act
            var result = await _controller.GetExpenseById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddNewExpense_ReturnsCreatedAtActionResult_WithExpense()
        {
            // Arrange
            var expense = new Expense { Id = Guid.NewGuid() };
            _mockRepo.Setup(repo => repo.AddNewExpense(expense)).ReturnsAsync(expense);

            // Act
            var result = await _controller.AddNewExpense(expense);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(expense, createdAtActionResult.Value);
        }

        // Add similar tests for UpdateExpense and DeleteExpense

        private List<ExpenseDto> GetTestExpenses()
        {
            return new List<ExpenseDto>
            {
                new ExpenseDto {  Amount = 100 },
                new ExpenseDto {  Amount = 200 }
            };
        }
    }
}