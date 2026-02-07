// tests/Faraday.Application.Tests/UseCases/Transactions/CreateTransactionUseCaseTests.cs

using Faraday.Application.DTOs.Transactions;
using Faraday.Application.Interfaces;
using Faraday.Application.UseCases.Transactions;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Moq;

namespace Faraday.Application.Tests.UseCases.Transactions;

public class CreateTransactionUseCaseTests {
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly CreateTransactionUseCase _useCase;

    public CreateTransactionUseCaseTests() {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockAccountRepository = new Mock<IAccountRepository>();
        _useCase = new CreateTransactionUseCase(
            _mockTransactionRepository.Object,
            _mockAccountRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_CreatesTransaction() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = CreateAccountWithId("Test Account", AccountType.Checking, 1000m, accountId);

        CreateTransactionRequest request = new() {
            Date = DateTime.UtcNow,
            Amount = 500m,
            Description = "Salary",
            AccountId = accountId,
            CategoryId = Guid.NewGuid()
        };

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        _mockTransactionRepository
            .Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
            .ReturnsAsync((Transaction t) => t);

        // Act
        TransactionDto result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500m, result.Amount);
        Assert.Equal("Salary", result.Description);
        Assert.Equal(accountId, result.AccountId);
        Assert.Equal("Test Account", result.AccountName);

        // Verify repository was called
        _mockTransactionRepository.Verify(
            r => r.CreateAsync(It.Is<Transaction>(t =>
                t.Amount == 500m &&
                t.Description == "Salary")),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentAccount_ThrowsException() {
        // Arrange
        CreateTransactionRequest request = new() {
            Date = DateTime.UtcNow,
            Amount = 500m,
            Description = "Test",
            AccountId = Guid.NewGuid()
        };

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(request.AccountId))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(request));

        Assert.Contains("not found", exception.Message);

        _mockTransactionRepository.Verify(
            r => r.CreateAsync(It.IsAny<Transaction>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithInactiveAccount_ThrowsException() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = CreateAccountWithId("Inactive Account", AccountType.Checking, 1000m, accountId);
        account.Deactivate();

        CreateTransactionRequest request = new() {
            Date = DateTime.UtcNow,
            Amount = 500m,
            Description = "Test",
            AccountId = accountId
        };

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(request));

        Assert.Contains("inactive", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyDescription_ThrowsException() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = CreateAccountWithId("Test Account", AccountType.Checking, 1000m, accountId);

        CreateTransactionRequest request = new() {
            Date = DateTime.UtcNow,
            Amount = 500m,
            Description = "", // Empty
            AccountId = accountId
        };

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(request));
    }

    private Account CreateAccountWithId(string name, AccountType type, decimal openingBalance, Guid id) {
        Account account = new(name, type, openingBalance);
        typeof(Account).GetProperty(nameof(Account.Id))!
            .SetValue(account, id);
        return account;
    }
}