// src/Faraday.Application/UseCases/Transactions/VoidTransactionUseCase.cs

using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.Application.UseCases.Transactions;

/// <summary>
/// Use case for voiding (soft deleting) a transaction
/// </summary>
public class VoidTransactionUseCase {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly ITransactionRepository _transactionRepository;


    // =========== //
    // Constructor //
    // =========== //
    public VoidTransactionUseCase(ITransactionRepository transactionRepository) {
        _transactionRepository = transactionRepository;
    }


    // ======== //
    // Function //
    // ======== //
    public async Task ExecuteAsync(Guid transactionId) {
        // Get transaction
        Transaction? transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null) {
            throw new InvalidOperationException(
                $"Transaction with ID {transactionId} not found");
        }

        // Void using domain method (validates)
        transaction.Void();

        // Persist changes
        await _transactionRepository.UpdateAsync(transaction);
    }
}