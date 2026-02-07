// tests/Faraday.Application.Tests/UseCases/Accounts/GetAllAccountsUseCaseTests.cs

using Faraday.Application.DTOs.Accounts;
using Faraday.Application.Interfaces;
using Faraday.Application.UseCases.Accounts;
using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Moq;

namespace Faraday.Application.Tests.UseCases.Accounts;

public class GetAllAccountsUseCaseTests {
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly GetAllAccountsUseCase _useCase;

    public GetAllAccountsUseCaseTests() {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _useCase = new GetAllAccountsUseCase(
            _mockAccountRepository.Object,
            _mockTransactionRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithActiveAccountsOnly_ReturnsOnlyActiveAccounts() {
        // Arrange
        Account account1 = CreateAccountWithId("Active Account", AccountType.Checking, 1000m);
        Account account2 = CreateAccountWithId("Inactive Account", AccountType.Savings, 2000m);
        account2.Deactivate();

        _mockAccountRepository
            .Setup(r => r.GetAllActiveAsync())
            .ReturnsAsync(new List<Account> { account1 });

        _mockTransactionRepository
            .Setup(r => r.GetActiveByAccountIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Transaction>());

        // Act
        IEnumerable<AccountDto> result = await _useCase.ExecuteAsync(includeInactive: false);

        // Assert
        Assert.Single(result);
        Assert.All(result, a => Assert.True(a.IsActive));
    }

    [Fact]
    public async Task ExecuteAsync_WithIncludeInactive_ReturnsAllAccounts() {
        // Arrange
        Account account1 = CreateAccountWithId("Active Account", AccountType.Checking, 1000m);
        Account account2 = CreateAccountWithId("Inactive Account", AccountType.Savings, 2000m);
        account2.Deactivate();

        _mockAccountRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Account> { account1, account2 });

        _mockTransactionRepository
            .Setup(r => r.GetActiveByAccountIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Transaction>());

        // Act
        IEnumerable<AccountDto> result = await _useCase.ExecuteAsync(includeInactive: true);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task ExecuteAsync_CalculatesBalanceForEachAccount() {
        // Arrange
        Account account1 = CreateAccountWithId("Account 1", AccountType.Checking, 1000m);
        Account account2 = CreateAccountWithId("Account 2", AccountType.Savings, 2000m);

        List<Transaction> transactions1 = [
            new(DateTime.UtcNow, 500m, "Income", account1.Id)
        ];

        List<Transaction> transactions2 = [
            new(DateTime.UtcNow, -300m, "Expense", account2.Id)
        ];

        _mockAccountRepository
            .Setup(r => r.GetAllActiveAsync())
            .ReturnsAsync(new List<Account> { account1, account2 });

        _mockTransactionRepository
            .Setup(r => r.GetActiveByAccountIdAsync(account1.Id))
            .ReturnsAsync(transactions1);

        _mockTransactionRepository
            .Setup(r => r.GetActiveByAccountIdAsync(account2.Id))
            .ReturnsAsync(transactions2);

        // Act
        IEnumerable<AccountDto> result = await _useCase.ExecuteAsync();

        // Assert
        List<AccountDto> resultList = result.ToList();
        Assert.Equal(2, resultList.Count);

        AccountDto acc1Result = resultList.First(a => a.Name == "Account 1");
        Assert.Equal(1500m, acc1Result.CurrentBalance); // 1000 + 500

        AccountDto acc2Result = resultList.First(a => a.Name == "Account 2");
        Assert.Equal(1700m, acc2Result.CurrentBalance); // 2000 - 300
    }

    private Account CreateAccountWithId(string name, AccountType type, decimal openingBalance) {
        Account account = new(name, type, openingBalance);
        typeof(Account).GetProperty(nameof(Account.Id))!
            .SetValue(account, Guid.NewGuid());
        return account;
    }
}