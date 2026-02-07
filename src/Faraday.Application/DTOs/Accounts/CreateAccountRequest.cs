// src/Faraday.Application/DTOs/CreateAccountRequest.cs

using Faraday.Domain.Enums;

namespace Faraday.Application.DTOs.Accounts;

/// <summary>
/// Request DTO for creating a new account
/// </summary>
public class CreateAccountRequest {
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public decimal OpeningBalance { get; set; }
    public CurrencyType Currency { get; set; } = CurrencyType.Usd;
}