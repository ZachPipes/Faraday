// tests/Faraday.Domain.Tests/Entities/TransactionTests.cs

using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Faraday.Domain.Exceptions;

namespace Faraday.Domain.Tests.Entities;

public class TransactionTests {
    [Fact]
    public void Constructor_WithPositiveAmount_CreatesIncomeTransaction() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        DateTime date = DateTime.UtcNow;

        // Act
        Transaction transaction = new(date, 500m, "Salary", 0, accountId);

        // Assert
        Assert.Equal(500m, transaction.Amount);
        Assert.Equal(TransactionType.Income, transaction.Type);
        Assert.True(transaction.IsIncome());
    }

    [Fact]
    public void Constructor_WithNegativeAmount_CreatesExpenseTransaction() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        DateTime date = DateTime.UtcNow;

        // Act
        Transaction transaction = new(date, -100m, "Groceries", 0, accountId);

        // Assert
        Assert.Equal(-100m, transaction.Amount);
        Assert.Equal(TransactionType.Expense, transaction.Type);
        Assert.True(transaction.IsExpense());
    }

    [Fact]
    public void Constructor_WithEmptyDescription_ThrowsException() {
        // Arrange
        Guid accountId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Transaction(DateTime.UtcNow, 100m, "", 0, accountId));
    }

    [Fact]
    public void Void_WhenNotVoided_VoidsTransaction() {
        // Arrange
        Transaction transaction = new(DateTime.UtcNow, 100m, "Test", 0, Guid.NewGuid());

        // Act
        transaction.Void();

        // Assert
        Assert.True(transaction.IsVoid);
    }

    [Fact]
    public void Void_WhenAlreadyVoided_ThrowsException() {
        // Arrange
        Transaction transaction = new(DateTime.UtcNow, 100m, "Test", 0, Guid.NewGuid());
        transaction.Void();

        // Act & Assert
        Assert.Throws<InvalidTransactionException>(() => transaction.Void());
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_UpdatesDescription() {
        // Arrange
        Transaction transaction = new(DateTime.UtcNow, 100m, "Old",  0,Guid.NewGuid());

        // Act
        transaction.UpdateDescription("New Description");

        // Assert
        Assert.Equal("New Description", transaction.Description);
    }
}