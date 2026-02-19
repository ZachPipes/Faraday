// tests/Faraday.Application.Tests/UseCases/Accounts/GetAccountByIdUseCaseTests.cs

using Faraday.Application.DTOs.Accounts;
using Faraday.Application.Interfaces;
using Faraday.Application.UseCases.Accounts;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Moq;

namespace Faraday.Application.Tests.UseCases.Accounts;

public class GetAccountByIdUseCaseTests {
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly GetAccountByIdUseCase _useCase;

    public GetAccountByIdUseCaseTests() {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _useCase = new GetAccountByIdUseCase(
            _mockAccountRepository.Object,
            _mockTransactionRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingAccount_ReturnsAccountWithBalance() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = new("Test Account", AccountType.Checking, 1000m);

        // Use reflection to set the ID (since it's init-only)
        typeof(Account).GetProperty(nameof(Account.Id))!
            .SetValue(account, accountId);

        List<Transaction> transactions = [
            new(DateTime.UtcNow, 500m, "Income", 0, accountId),
            new(DateTime.UtcNow, -200m, "Expense", 0, accountId)
        ];

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        _mockTransactionRepository
            .Setup(r => r.GetActiveByAccountIdAsync(accountId))
            .ReturnsAsync(transactions);

        // Act
        AccountDto? result = await _useCase.ExecuteAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountId, result.Id);
        Assert.Equal("Test Account", result.Name);
        Assert.Equal(1300m, result.CurrentBalance); // 1000 + 500 - 200
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentAccount_ReturnsNull() {
        // Arrange
        Guid accountId = Guid.NewGuid();

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync((Account?)null);

        // Act
        AccountDto? result = await _useCase.ExecuteAsync(accountId);

        // Assert
        Assert.Null(result);

        // Verify transactions were never queried
        _mockTransactionRepository.Verify(
            r => r.GetActiveByAccountIdAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoTransactions_ReturnsOpeningBalance() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = new("Test Account", AccountType.Checking, 1000m);

        typeof(Account).GetProperty(nameof(Account.Id))!
            .SetValue(account, accountId);

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        _mockTransactionRepository
            .Setup(r => r.GetActiveByAccountIdAsync(accountId))
            .ReturnsAsync(new List<Transaction>());

        // Act
        AccountDto? result = await _useCase.ExecuteAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000m, result.CurrentBalance);
    }
}