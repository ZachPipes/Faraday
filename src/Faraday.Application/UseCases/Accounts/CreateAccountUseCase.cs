// src/Faraday.Application/UseCases/Accounts/CreateAccountUseCase.cs

using Faraday.Application.DTOs.Accounts;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.Application.UseCases.Accounts;

/// <summary>
/// Use case for creating a new account
/// </summary>
public class CreateAccountUseCase {
    private readonly IAccountRepository _accountRepository;

    public CreateAccountUseCase(IAccountRepository accountRepository) {
        _accountRepository = accountRepository;
    }

    public async Task<AccountDto> ExecuteAsync(CreateAccountRequest request) {
        // Validate: Check if account name already exists
        bool exists = await _accountRepository.ExistsWithNameAsync(request.Name);
        if (exists) {
            throw new InvalidOperationException(
                $"An account with the name '{request.Name}' already exists");
        }

        // Create domain entity (business logic validates here)
        Account account = new(
            request.Name,
            request.Type,
            request.OpeningBalance,
            request.Currency);

        // Persist
        await _accountRepository.CreateAsync(account);

        // Return DTO
        return AccountDto.FromEntity(account, account.OpeningBalance);
    }
}