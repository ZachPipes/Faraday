// src/Faraday.Application/DTOs/Accounts/AccountDto.cs

using Faraday.Domain.Entities;
using Faraday.Domain.Enums;

namespace Faraday.Application.DTOs.Accounts;

/// <summary>
/// Account data transfer object for UI layer
/// </summary>
public class AccountDto {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public CurrencyType Currency { get; set; } = CurrencyType.Usd;
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Map from Domain Entity to DTO
    /// </summary>
    public static AccountDto FromEntity(Account account, decimal currentBalance) {
        return new AccountDto {
            Id = account.Id,
            Name = account.Name,
            Type = account.Type,
            Currency = account.Currency,
            OpeningBalance = account.OpeningBalance,
            CurrentBalance = currentBalance,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            ModifiedAt = account.ModifiedAt
        };
    }
}