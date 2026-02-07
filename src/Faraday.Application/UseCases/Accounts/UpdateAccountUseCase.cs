// src/Faraday.Application/UseCases/Accounts/UpdateAccountUseCase.cs

using Faraday.Application.DTOs.Accounts;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.Application.UseCases.Accounts;

/// <summary>
/// Use case for updating an account's name
/// </summary>
public class UpdateAccountUseCase {
    private readonly IAccountRepository _accountRepository;

    public UpdateAccountUseCase(IAccountRepository accountRepository) {
        _accountRepository = accountRepository;
    }

    public async Task<AccountDto> ExecuteAsync(UpdateAccountRequest request) {
        // Get existing account
        Account? account = await _accountRepository.GetByIdAsync(request.Id);
        if (account == null) {
            throw new InvalidOperationException(
                $"Account with ID {request.Id} not found");
        }

        // Update using domain method (validates)
        account.UpdateName(request.Name);

        // Persist changes
        await _accountRepository.UpdateAsync(account);

        // Return updated DTO
        return AccountDto.FromEntity(account, account.OpeningBalance);
    }
}