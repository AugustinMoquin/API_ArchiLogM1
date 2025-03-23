using System.Collections.Generic;
using System.Threading.Tasks;
using exchangeRateApi.Controllers;
using exchangeRateApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace exchangeRateApi.Tests
{
    public class CurrencyControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<ILogger<CurrencyController>> _mockLogger;
        private readonly CurrencyController _controller;

        public CurrencyControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<ApplicationDbContext>(options);
            _mockLogger = new Mock<ILogger<CurrencyController>>();
            _controller = new CurrencyController(_mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCurrency_ReturnsNotFound_WhenCurrencyDoesNotExist()
        {
            // Arrange
            int testCurrencyId = 1;
            _mockContext.Setup(c => c.Currencies.FindAsync(testCurrencyId))
                .ReturnsAsync((Currency)null);

            // Act
            var result = await _controller.GetCurrency(testCurrencyId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCurrency_ReturnsCurrency_WhenCurrencyExists()
        {
            // Arrange
            int testCurrencyId = 1;
            var testCurrency = new Currency { Id = testCurrencyId, Currency_Name = "Test Currency" };
            _mockContext.Setup(c => c.Currencies.FindAsync(testCurrencyId))
                .ReturnsAsync(testCurrency);

            // Act
            var result = await _controller.GetCurrency(testCurrencyId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Currency>>(result);
            var returnValue = Assert.IsType<Currency>(actionResult.Value);
            Assert.Equal(testCurrencyId, returnValue.Id);
        }
    }
}