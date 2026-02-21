using Faraday.Domain.Entities;
using Faraday.Domain.Exceptions;

namespace Faraday.Domain.Tests.Entities;

public class StockTests {
    [Fact]
    public void Constructor_WithNegativeAmount_CreatesExpenseStock() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        DateTime date = DateTime.UtcNow;

        // Act
        Stock stock = new Stock(
            DateTime.UtcNow,
            "BUY",
            "GROC",
            "Groceries",
            "Equity",
            100m,
            1m,
            0m,
            0m,
            0m,
            -100m,
            0m,
            DateTime.UtcNow,
            Guid.NewGuid()
        );

        // Assert
        Assert.Equal(-100m, stock.Amount);
    }

    [Fact]
    public void Constructor_WithEmptyDescription_ThrowsException() {
        // Arrange
        Guid accountId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Stock(
                DateTime.UtcNow,
                "BUY",
                "GROC",
                "Groceries",
                "Equity",
                100m,
                1m,
                0m,
                0m,
                0m,
                -100m,
                0m,
                DateTime.UtcNow,
                Guid.NewGuid()
            ));
    }

    [Fact]
    public void Void_WhenNotVoided_VoidsStock() {
        // Arrange
        Stock stock = new Stock(
            DateTime.UtcNow,
            "BUY",
            "GROC",
            "Groceries",
            "Equity",
            100m,
            1m,
            0m,
            0m,
            0m,
            -100m,
            0m,
            DateTime.UtcNow,
            Guid.NewGuid()
        );

        // Act
        stock.Void();

        // Assert
        Assert.True(stock.IsVoid);
    }

    [Fact]
    public void Void_WhenAlreadyVoided_ThrowsException() {
        // Arrange
        Stock stock = new Stock(
            DateTime.UtcNow,
            "BUY",
            "GROC",
            "Groceries",
            "Equity",
            100m,
            1m,
            0m,
            0m,
            0m,
            -100m,
            0m,
            DateTime.UtcNow,
            Guid.NewGuid()
        );
        stock.Void();

        // Act & Assert
        Assert.Throws<InvalidStockException>(() => stock.Void());
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_UpdatesDescription() {
        // Arrange
        Stock stock = new Stock(
            DateTime.UtcNow,
            "BUY",
            "GROC",
            "Groceries",
            "Equity",
            100m,
            1m,
            0m,
            0m,
            0m,
            -100m,
            0m,
            DateTime.UtcNow,
            Guid.NewGuid()
        );

        // Act
        stock.UpdateDescription("New Description");

        // Assert
        Assert.Equal("New Description", stock.Description);
    }
}