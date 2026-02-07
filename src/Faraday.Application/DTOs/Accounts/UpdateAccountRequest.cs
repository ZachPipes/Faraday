// src/Faraday.Application/DTOs/Accounts/UpdateAccountRequest.cs

namespace Faraday.Application.DTOs.Accounts;

/// <summary>
/// Request DTO for updating an account
/// </summary>
public class UpdateAccountRequest {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}