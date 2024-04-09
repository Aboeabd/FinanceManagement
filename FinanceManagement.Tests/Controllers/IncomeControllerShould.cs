using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceManagement.Api.Controllers;
using FinanceManagement.Api.Controllers.Interfaces;
using FinanceManagement.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FinanceManagement.Api.Tests
{
    public class IncomeControllerShould
    {
        private readonly Mock<IIncomeRepository> _mockRepo;
        private readonly IncomeController _controller;

        public IncomeControllerShould()
        {
            _mockRepo = new Mock<IIncomeRepository>();
            _controller = new IncomeController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllIncome_ReturnsOkResult_WithListOfIncome()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAllIncome()).ReturnsAsync(GetTestIncomes());

            // Act
            var result = await _controller.GetAllIncome();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsType<List<IncomeComplete>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public async Task GetIncomeById_ReturnsNotFoundResult_WhenIdDoesNotExist()
        {
            // Arrange
            _mockRepo
                .Setup(repo => repo.GetIncomeById(It.IsAny<Guid>()))
                .ReturnsAsync((IncomeComplete)null);

            // Act
            var result = await _controller.GetIncomeById(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddNewIncome_ReturnsCreatedAtActionResult_WithIncome()
        {
            // Arrange
            var income = new Income
            {
                IncomeType = IncomeType.Salary,
                Amount = 1000,
                OneTimeIncomeDate = DateTime.Now
            };
            var incomeComplete = new IncomeComplete
            {
                Id = Guid.NewGuid(),
                IncomeType = income.IncomeType,
                Amount = income.Amount,
                OneTimeIncomeDate = income.OneTimeIncomeDate
            };
            _mockRepo.Setup(repo => repo.AddNewIncome(income)).ReturnsAsync(incomeComplete);

            // Act
            var result = await _controller.AddNewIncome(income);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedIncomeComplete = Assert.IsType<IncomeComplete>(createdAtActionResult.Value);
            Assert.Equal(incomeComplete.Id, returnedIncomeComplete.Id);
            Assert.Equal(income.Amount, returnedIncomeComplete.Amount);
            Assert.Equal(income.IncomeType, returnedIncomeComplete.IncomeType);
            Assert.Equal(income.OneTimeIncomeDate, returnedIncomeComplete.OneTimeIncomeDate);
        }

        // Add similar tests for UpdateIncome and DeleteIncome

        private List<IncomeComplete> GetTestIncomes()
        {
            return new List<IncomeComplete>
            {
                new IncomeComplete { Id = Guid.NewGuid(), Amount = 100 },
                new IncomeComplete { Id = Guid.NewGuid(), Amount = 200 }
            };
        }
    }
}
