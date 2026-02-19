// tests/Faraday.Application.Tests/UseCases/Transactions/GetTransactionsByAccountUseCaseTests.cs

using Faraday.Application.DTOs.Transactions;
using Faraday.Application.Interfaces;
using Faraday.Application.UseCases.Transactions;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Moq;

namespace Faraday.Application.Tests.UseCases.Transactions;

public class GetTransactionsByAccountUseCaseTests {
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly GetTransactionsByAccountUseCase _useCase;

    public GetTransactionsByAccountUseCaseTests() {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockAccountRepository = new Mock<IAccountRepository>();
        _useCase = new GetTransactionsByAccountUseCase(
            _mockTransactionRepository.Object,
            _mockAccountRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingAccount_ReturnsTransactions() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = CreateAccountWithId("Test Account", AccountType.Checking, 1000m, accountId);

        List<Transaction> transactions = [
            new(DateTime.UtcNow.AddDays(-2), 500m, "Income", 0, accountId),
            new(DateTime.UtcNow.AddDays(-1), -200m, "Expense", 0, accountId)
        ];

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        _mockTransactionRepository
            .Setup(r => r.GetActiveByAccountIdAsync(accountId))
            .ReturnsAsync(transactions);

        // Act
        IEnumerable<TransactionDto> result = await _useCase.ExecuteAsync(accountId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Equal("Test Account", t.AccountName));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsTransactionsOrderedByDateDescending() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = CreateAccountWithId("Test Account", AccountType.Checking, 1000m, accountId);

        List<Transaction> transactions = [
            new(DateTime.UtcNow.AddDays(-5), 100m, "Oldest", 0, accountId),
            new(DateTime.UtcNow.AddDays(-1), 200m, "Recent", 0, accountId),
            new(DateTime.UtcNow.AddDays(-3), 150m, "Middle", 0, accountId)
        ];

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        _mockTransactionRepository
            .Setup(r => r.GetActiveByAccountIdAsync(accountId))
            .ReturnsAsync(transactions);

        // Act
        IEnumerable<TransactionDto> result = await _useCase.ExecuteAsync(accountId);

        // Assert
        List<TransactionDto> resultList = result.ToList();
        Assert.Equal("Recent", resultList[0].Description);
        Assert.Equal("Middle", resultList[1].Description);
        Assert.Equal("Oldest", resultList[2].Description);
    }

    [Fact]
    public async Task ExecuteAsync_WithIncludeVoidedFalse_ExcludesVoidedTransactions() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = CreateAccountWithId("Test Account", AccountType.Checking, 1000m, accountId);

        Transaction activeTransaction = new(DateTime.UtcNow, 100m, "Active", 0, accountId);
        Transaction voidedTransaction = new(DateTime.UtcNow, 200m, "Voided", 0, accountId);
        voidedTransaction.Void();

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        _mockTransactionRepository
            .Setup(r => r.GetActiveByAccountIdAsync(accountId))
            .ReturnsAsync(new List<Transaction> { activeTransaction });

        // Act
        IEnumerable<TransactionDto> result = await _useCase.ExecuteAsync(accountId, includeVoided: false);

        // Assert
        Assert.Single(result);
        Assert.Equal("Active", result.First().Description);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentAccount_ThrowsException() {
        // Arrange
        Guid accountId = Guid.NewGuid();

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(accountId));
    }

    private Account CreateAccountWithId(string name, AccountType type, decimal openingBalance, Guid id) {
        Account account = new(name, type, openingBalance);
        typeof(Account).GetProperty(nameof(Account.Id))!
            .SetValue(account, id);
        return account;
    }
}