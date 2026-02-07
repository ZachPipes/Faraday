// src/Faraday.Application/DTOs/Transactions/TransactionDto.cs

using Faraday.Domain.Entities;
using Faraday.Domain.Enums;

namespace Faraday.Application.DTOs.Transactions;

/// <summary>
/// Transaction data transfer object for UI layer
/// </summary>
public class TransactionDto {
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public Guid AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public bool IsVoid { get; set; }
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Map from Domain Entity to DTO
    /// </summary>
    public static TransactionDto FromEntity(Transaction transaction, string accountName = "") {
        return new TransactionDto {
            Id = transaction.Id,
            Date = transaction.Date,
            Amount = transaction.Amount,
            Description = transaction.Description,
            Type = transaction.Type,
            AccountId = transaction.AccountId,
            AccountName = accountName,
            IsVoid = transaction.IsVoid,
            CreatedAt = transaction.CreatedAt
        };
    }
}