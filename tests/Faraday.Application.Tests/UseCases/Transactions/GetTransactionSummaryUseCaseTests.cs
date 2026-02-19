// tests/Faraday.Application.Tests/UseCases/Transactions/GetTransactionSummaryUseCaseTests.cs

using Faraday.Application.DTOs.Transactions;
using Faraday.Application.Interfaces;
using Faraday.Application.UseCases.Transactions;
using Faraday.Domain.Entities;
using Faraday.Domain.ValueObjects;
using Moq;

namespace Faraday.Application.Tests.UseCases.Transactions;

public class GetTransactionSummaryUseCaseTests {
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly GetTransactionSummaryUseCase _useCase;

    public GetTransactionSummaryUseCaseTests() {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _useCase = new GetTransactionSummaryUseCase(_mockTransactionRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_CalculatesCorrectSummary() {
        // Arrange
        DateRange dateRange = DateRange.ForMonth(2024, 1);
        Guid accountId = Guid.NewGuid();

        List<Transaction> transactions = [
            new(DateTime.UtcNow, 1000m, "Salary", 0, accountId), // Income
            new(DateTime.UtcNow, 500m, "Bonus", 0, accountId), // Income
            new(DateTime.UtcNow, -200m, "Groceries", 0, accountId), // Expense
            new(DateTime.UtcNow, -150m, "Gas", 0, accountId)
        ];

        _mockTransactionRepository
            .Setup(r => r.GetByDateRangeAsync(dateRange))
            .ReturnsAsync(transactions);

        // Act
        TransactionSummaryDto result = await _useCase.ExecuteAsync(dateRange);

        // Assert
        Assert.Equal(1500m, result.TotalIncome); // 1000 + 500
        Assert.Equal(350m, result.TotalExpenses); // 200 + 150 (positive)
        Assert.Equal(1150m, result.NetAmount); // 1500 - 350
        Assert.Equal(4, result.TransactionCount);
    }

    [Fact]
    public async Task ExecuteAsync_ExcludesVoidedTransactions() {
        // Arrange
        DateRange dateRange = DateRange.ForMonth(2024, 1);
        Guid accountId = Guid.NewGuid();

        Transaction activeTransaction = new(DateTime.UtcNow, 1000m, "Active", 0, accountId);
        Transaction voidedTransaction = new(DateTime.UtcNow, 500m, "Voided", 0, accountId);
        voidedTransaction.Void();

        List<Transaction> transactions = [
            activeTransaction, 
            voidedTransaction
        ];

        _mockTransactionRepository
            .Setup(r => r.GetByDateRangeAsync(dateRange))
            .ReturnsAsync(transactions);

        // Act
        TransactionSummaryDto result = await _useCase.ExecuteAsync(dateRange);

        // Assert
        Assert.Equal(1000m, result.TotalIncome);
        Assert.Equal(1, result.TransactionCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoTransactions_ReturnsZeroSummary() {
        // Arrange
        DateRange dateRange = DateRange.ForMonth(2024, 1);

        _mockTransactionRepository
            .Setup(r => r.GetByDateRangeAsync(dateRange))
            .ReturnsAsync(new List<Transaction>());

        // Act
        TransactionSummaryDto result = await _useCase.ExecuteAsync(dateRange);

        // Assert
        Assert.Equal(0m, result.TotalIncome);
        Assert.Equal(0m, result.TotalExpenses);
        Assert.Equal(0m, result.NetAmount);
        Assert.Equal(0, result.TransactionCount);
    }
}