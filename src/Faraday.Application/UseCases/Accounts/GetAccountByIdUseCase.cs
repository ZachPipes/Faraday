// src/Faraday.Application/UseCases/Accounts/GetAccountByIdUseCase.cs

using Faraday.Application.DTOs.Accounts;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.Application.UseCases.Accounts;

/// <summary>
/// Use case for retrieving a single account with its current balance
/// </summary>
public class GetAccountByIdUseCase {
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public GetAccountByIdUseCase(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository) {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<AccountDto?> ExecuteAsync(Guid accountId) {
        // Get account
        Account? account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
            return null;

        // Get transactions to calculate current balance
        IEnumerable<Transaction> transactions = await _transactionRepository
            .GetActiveByAccountIdAsync(accountId);

        // Use domain logic to calculate balance
        decimal currentBalance = account.CalculateCurrentBalance(transactions);

        // Return DTO
        return AccountDto.FromEntity(account, currentBalance);
    }
}