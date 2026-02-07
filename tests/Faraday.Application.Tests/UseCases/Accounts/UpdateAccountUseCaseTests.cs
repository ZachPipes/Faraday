// tests/Faraday.Application.Tests/UseCases/Accounts/UpdateAccountUseCaseTests.cs

using Faraday.Application.DTOs.Accounts;
using Faraday.Application.Interfaces;
using Faraday.Application.UseCases.Accounts;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Moq;

namespace Faraday.Application.Tests.UseCases.Accounts;

public class UpdateAccountUseCaseTests {
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly UpdateAccountUseCase _useCase;

    public UpdateAccountUseCaseTests() {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _useCase = new UpdateAccountUseCase(_mockAccountRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_UpdatesAccountName() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = new("Old Name", AccountType.Checking, 1000m);
        typeof(Account).GetProperty(nameof(Account.Id))!
            .SetValue(account, accountId);

        UpdateAccountRequest request = new() {
            Id = accountId,
            Name = "New Name"
        };

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        _mockAccountRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Account>()))
            .Returns(Task.CompletedTask);

        // Act
        AccountDto result = await _useCase.ExecuteAsync(request);

        // Assert
        Assert.Equal("New Name", result.Name);
        Assert.Equal(accountId, result.Id);

        // Verify update was called
        _mockAccountRepository.Verify(
            r => r.UpdateAsync(It.Is<Account>(a => a.Name == "New Name")),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentAccount_ThrowsException() {
        // Arrange
        UpdateAccountRequest request = new() {
            Id = Guid.NewGuid(),
            Name = "New Name"
        };

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync((Account?)null);

        // Act & Assert
        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(request));

        Assert.Contains("not found", exception.Message);

        // Verify update was never called
        _mockAccountRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Account>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyName_ThrowsException() {
        // Arrange
        Guid accountId = Guid.NewGuid();
        Account account = new Account("Old Name", AccountType.Checking, 1000m);
        typeof(Account).GetProperty(nameof(Account.Id))!
            .SetValue(account, accountId);

        UpdateAccountRequest request = new() {
            Id = accountId,
            Name = "" // Empty name
        };

        _mockAccountRepository
            .Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(request));
    }
}