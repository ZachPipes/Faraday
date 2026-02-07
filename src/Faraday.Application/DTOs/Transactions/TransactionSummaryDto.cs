// src/Faraday.Application/DTOs/Transactions/TransactionSummaryDto.cs

namespace Faraday.Application.DTOs.Transactions;

/// <summary>
/// Summary statistics for transactions (used in reports)
/// </summary>
public class TransactionSummaryDto {
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetAmount { get; set; }
    public int TransactionCount { get; set; }
}