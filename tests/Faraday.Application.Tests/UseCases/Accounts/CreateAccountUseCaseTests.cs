// tests/Faraday.Application.Tests/UseCases/Accounts/CreateAccountUseCaseTests.cs

using Faraday.Application.DTOs.Accounts;
using Faraday.Application.Interfaces;
using Faraday.Application.UseCases.Accounts;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Moq;

namespace Faraday.Application.Tests.UseCases.Accounts;

public class CreateAccountUseCaseTests {
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly CreateAccountUseCase _useCase;

    public CreateAccountUseCaseTests() {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _useCase = new CreateAccountUseCase(_mockAccountRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_CreatesAccount() {
        // Arrange
        CreateAccountRequest request = new() {
            Name = "Test Checking",
            Type = AccountType.Checking,
            OpeningBalance = 1000m,
            Currency = CurrencyType.Usd
        };

        _mockAccountRepository
            .Setup(r => r.ExistsWithNameAsync(request.Name))
            .ReturnsAsync(false);

        _mockAccountRepository
            .Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ReturnsAsync((Account a) => a);

        // Act
        AccountDto result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Checking", result.Name);
        Assert.Equal(AccountType.Checking, result.Type);
        Assert.Equal(1000m, result.OpeningBalance);
        Assert.Equal(CurrencyType.Usd, result.Currency);
        Assert.True(result.IsActive);

        // Verify repository was called
        _mockAccountRepository.Verify(
            r => r.CreateAsync(It.Is<Account>(a =>
                a.Name == "Test Checking" &&
                a.Type == AccountType.Checking)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateName_ThrowsException() {
        // Arrange
        CreateAccountRequest request = new() {
            Name = "Existing Account",
            Type = AccountType.Checking,
            OpeningBalance = 1000m
        };

        _mockAccountRepository
            .Setup(r => r.ExistsWithNameAsync(request.Name))
            .ReturnsAsync(true);

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(request));

        Assert.Contains("already exists", exception.Message);

        // Verify CreateAsync was never called
        _mockAccountRepository.Verify(
            r => r.CreateAsync(It.IsAny<Account>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidName_ThrowsException() {
        // Arrange
        CreateAccountRequest request = new() {
            Name = "", // Empty name
            Type = AccountType.Checking,
            OpeningBalance = 1000m
        };

        _mockAccountRepository
            .Setup(r => r.ExistsWithNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_WithNegativeOpeningBalance_AllowsIt() {
        // Arrange - Credit cards can have negative opening balance
        CreateAccountRequest request = new() {
            Name = "Credit Card",
            Type = AccountType.CreditCard,
            OpeningBalance = -500m,
            Currency = CurrencyType.Usd
        };

        _mockAccountRepository
            .Setup(r => r.ExistsWithNameAsync(request.Name))
            .ReturnsAsync(false);

        _mockAccountRepository
            .Setup(r => r.CreateAsync(It.IsAny<Account>()))
            .ReturnsAsync((Account a) => a);

        // Act
        AccountDto result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.Equal(-500m, result.OpeningBalance);
    }
}