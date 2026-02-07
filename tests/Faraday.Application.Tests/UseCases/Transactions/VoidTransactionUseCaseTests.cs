// tests/Faraday.Application.Tests/UseCases/Transactions/VoidTransactionUseCaseTests.cs

using Faraday.Application.Interfaces;
using Faraday.Application.UseCases.Transactions;
using Faraday.Domain.Entities;
using Moq;

namespace Faraday.Application.Tests.UseCases.Transactions;

public class VoidTransactionUseCaseTests {
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly VoidTransactionUseCase _useCase;

    public VoidTransactionUseCaseTests() {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _useCase = new VoidTransactionUseCase(_mockTransactionRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidTransaction_VoidsTransaction() {
        // Arrange
        Guid transactionId = Guid.NewGuid();
        Transaction transaction = new(DateTime.UtcNow, 100m, "Test", Guid.NewGuid());
        typeof(Transaction).GetProperty(nameof(Transaction.Id))!
            .SetValue(transaction, transactionId);

        _mockTransactionRepository
            .Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(transaction);

        _mockTransactionRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
            .Returns(Task.CompletedTask);

        // Act
        await _useCase.ExecuteAsync(transactionId);

        // Assert
        Assert.True(transaction.IsVoid);

        _mockTransactionRepository.Verify(
            r => r.UpdateAsync(It.Is<Transaction>(t => t.IsVoid)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentTransaction_ThrowsException() {
        // Arrange
        Guid transactionId = Guid.NewGuid();

        _mockTransactionRepository
            .Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync((Transaction?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.ExecuteAsync(transactionId));

        _mockTransactionRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Transaction>()),
            Times.Never);
    }
}