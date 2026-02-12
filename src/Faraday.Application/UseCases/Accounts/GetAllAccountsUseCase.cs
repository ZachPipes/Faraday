// src/Faraday.Application/UseCases/Accounts/GetAllAccountsUseCase.cs

using Faraday.Application.DTOs.Accounts;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.Application.UseCases.Accounts;

/// <summary>
/// Use case for retrieving all accounts with their current balances
/// </summary>
public class GetAllAccountsUseCase {
    // ============ //
    // Dependencies //
    // ============ //
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    
    // =========== //
    // Constructor //
    // =========== //
    public GetAllAccountsUseCase(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository) {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    
    // ======== //
    // Function //
    // ======== //
    public async Task<IEnumerable<AccountDto>> ExecuteAsync(bool includeInactive = false) {
        // Get accounts
        IEnumerable<Account> accounts = includeInactive
            ? await _accountRepository.GetAllAsync()
            : await _accountRepository.GetAllActiveAsync();

        List<AccountDto> accountDtos = [];

        // Calculate balance for each account
        foreach (Account account in accounts) {
            IEnumerable<Transaction> transactions = await _transactionRepository
                .GetActiveByAccountIdAsync(account.Id);

            decimal currentBalance = account.CalculateCurrentBalance(transactions);

            accountDtos.Add(AccountDto.FromEntity(account, currentBalance));
        }

        return accountDtos;
    }
}