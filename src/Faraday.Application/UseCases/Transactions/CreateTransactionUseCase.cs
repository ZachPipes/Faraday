// src/Faraday.Application/UseCases/Transactions/CreateTransactionUseCase.cs

using Faraday.Application.DTOs.Transactions;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.Application.UseCases.Transactions;

/// <summary>
/// Use case for creating a new transaction
/// </summary>
public class CreateTransactionUseCase {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;

    
    // =========== //
    // Constructor //
    // =========== //
    public CreateTransactionUseCase(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository) {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
    }

    
    // ======== //
    // Function //
    // ======== //
    public async Task<TransactionDto> ExecuteAsync(CreateTransactionRequest request) {
        // Validate: Account must exist
        Account? account = await _accountRepository.GetByIdAsync(request.AccountId);
        if (account == null) {
            throw new InvalidOperationException(
                $"Account with ID {request.AccountId} not found");
        }

        // Validate: Account must be active
        if (!account.IsActive) {
            throw new InvalidOperationException(
                "Cannot create transactions for inactive accounts");
        }

        // Create domain entity (validates business rules)
        Transaction transaction = new(
            request.Date,
            request.Amount,
            request.Description,
            request.AccountId);

        // Validate transaction belongs to account (domain logic)
        account.ValidateTransaction(transaction);

        // Persist
        await _transactionRepository.CreateAsync(transaction);

        // Return DTO
        return TransactionDto.FromEntity(transaction, account.Name);
    }
}