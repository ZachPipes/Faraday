// src/Faraday.Application/DTOs/Transactions/CreateTransactionRequest.cs

namespace Faraday.Application.DTOs.Transactions;

/// <summary>
/// Request DTO for creating a new transaction
/// </summary>
public class CreateTransactionRequest {
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid AccountId { get; set; }
    public Guid? CategoryId { get; set; }
}