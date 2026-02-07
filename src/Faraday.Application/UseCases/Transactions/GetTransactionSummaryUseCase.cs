// src/Faraday.Application/UseCases/Transactions/GetTransactionSummaryUseCase.cs

using Faraday.Application.DTOs.Transactions;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;
using Faraday.Domain.ValueObjects;

namespace Faraday.Application.UseCases.Transactions;

/// <summary>
/// Use case for getting transaction summary statistics for a date range
/// </summary>
public class GetTransactionSummaryUseCase {
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionSummaryUseCase(ITransactionRepository transactionRepository) {
        _transactionRepository = transactionRepository;
    }

    public async Task<TransactionSummaryDto> ExecuteAsync(DateRange dateRange) {
        // Get transactions in date range
        IEnumerable<Transaction> transactions = await _transactionRepository.GetByDateRangeAsync(dateRange);

        // Filter out voided transactions
        List<Transaction> activeTransactions = transactions.Where(t => !t.IsVoid).ToList();

        // Calculate summary using domain logic
        decimal income = activeTransactions
            .Where(t => t.IsIncome())
            .Sum(t => t.Amount);

        decimal expenses = activeTransactions
            .Where(t => t.IsExpense())
            .Sum(t => Math.Abs(t.Amount)); // Make positive for display

        return new TransactionSummaryDto {
            TotalIncome = income,
            TotalExpenses = expenses,
            NetAmount = income - expenses,
            TransactionCount = activeTransactions.Count
        };
    }
}