// tests/Faraday.Domain.Tests/Entities/AccountTests.cs

using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Faraday.Domain.Exceptions;

namespace Faraday.Domain.Tests.Entities;

public class AccountTests {
    [Fact]
    public void Constructor_WithValidData_CreatesAccount() {
        // Arrange & Act
        Account account = new("Test Account", AccountType.Checking, 1000m);

        // Assert
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal("Test Account", account.Name);
        Assert.Equal(AccountType.Checking, account.Type);
        Assert.Equal(1000m, account.OpeningBalance);
        Assert.Equal(CurrencyType.Usd, account.Currency);
        Assert.True(account.IsActive);
    }

    [Fact]
    public void Constructor_WithEmptyName_ThrowsException() {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Account("", AccountType.Checking, 1000m));
    }

    [Fact]
    public void Constructor_WithLongName_ThrowsException() {
        // Arrange
        string longName = new('A', 101);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Account(longName, AccountType.Checking, 1000m));
    }

    [Fact]
    public void CalculateCurrentBalance_WithNoTransactions_ReturnsOpeningBalance() {
        // Arrange
        Account account = new("Test", AccountType.Checking, 1000m);
        
        // ReSharper disable once CollectionNeverUpdated.Local
        List<Transaction> transactions = [];

        // Act
        decimal balance = account.CalculateCurrentBalance(transactions);

        // Assert
        Assert.Equal(1000m, balance);
    }

    [Fact]
    public void CalculateCurrentBalance_WithTransactions_ReturnsCorrectSum() {
        // Arrange
        Account account = new("Test", AccountType.Checking, 1000m);
        List<Transaction> transactions = [
            new(DateTime.UtcNow, 500m, "Income", account.Id),
            new(DateTime.UtcNow, -200m, "Expense", account.Id)
        ];

        // Act
        decimal balance = account.CalculateCurrentBalance(transactions);

        // Assert
        Assert.Equal(1300m, balance); // 1000 + 500 - 200
    }

    [Fact]
    public void CalculateCurrentBalance_WithVoidedTransaction_ExcludesVoidedAmount() {
        // Arrange
        Account account = new("Test", AccountType.Checking, 1000m);
        Transaction transaction = new(DateTime.UtcNow, -100m, "Expense", account.Id);
        transaction.Void();

        List<Transaction> transactions = [transaction];

        // Act
        decimal balance = account.CalculateCurrentBalance(transactions);

        // Assert
        Assert.Equal(1000m, balance); // Voided transaction ignored
    }

    [Fact]
    public void UpdateName_WithValidName_UpdatesName() {
        // Arrange
        Account account = new("Old Name", AccountType.Checking, 1000m);
        DateTime originalModifiedAt = account.ModifiedAt;
        Thread.Sleep(10); // Ensure timestamp changes

        // Act
        account.UpdateName("New Name");

        // Assert
        Assert.Equal("New Name", account.Name);
        Assert.True(account.ModifiedAt > originalModifiedAt);
    }

    [Fact]
    public void Deactivate_WhenActive_DeactivatesAccount() {
        // Arrange
        Account account = new("Test", AccountType.Checking, 1000m);

        // Act
        account.Deactivate();

        // Assert
        Assert.False(account.IsActive);
    }

    [Fact]
    public void Deactivate_WhenAlreadyInactive_ThrowsException() {
        // Arrange
        Account account = new("Test", AccountType.Checking, 1000m);
        account.Deactivate();

        // Act & Assert
        Assert.Throws<InvalidAccountOperationException>(() => account.Deactivate());
    }
}