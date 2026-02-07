// src/Faraday.Application/UseCases/Transactions/GetTransactionsByAccountUseCase.cs

using Faraday.Application.DTOs.Transactions;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.Application.UseCases.Transactions;

/// <summary>
/// Use case for retrieving all transactions for a specific account
/// </summary>
public class GetTransactionsByAccountUseCase {
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;

    public GetTransactionsByAccountUseCase(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository) {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
    }

    public async Task<IEnumerable<TransactionDto>> ExecuteAsync(Guid accountId, bool includeVoided = false) {
        // Validate: Account exists
        Account? account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null) {
            throw new InvalidOperationException(
                $"Account with ID {accountId} not found");
        }

        // Get transactions
        IEnumerable<Transaction> transactions = includeVoided
            ? await _transactionRepository.GetByAccountIdAsync(accountId)
            : await _transactionRepository.GetActiveByAccountIdAsync(accountId);

        // Map to DTOs
        return transactions
            .Select(t => TransactionDto.FromEntity(t, account.Name))
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt);
    }
}