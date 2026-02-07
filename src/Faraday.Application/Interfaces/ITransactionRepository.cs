// src/Faraday.Application/Interfaces/ITransactionRepository.cs

using Faraday.Domain.Entities;
using Faraday.Domain.ValueObjects;

namespace Faraday.Application.Interfaces;

/// <summary>
/// Repository interface for Transaction persistence operations
/// </summary>
public interface ITransactionRepository {
    /// <summary>
    /// Get a transaction by its ID
    /// </summary>
    Task<Transaction?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all transactions for a specific account
    /// </summary>
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);

    /// <summary>
    /// Get all non-voided transactions for a specific account
    /// </summary>
    Task<IEnumerable<Transaction>> GetActiveByAccountIdAsync(Guid accountId);

    /// <summary>
    /// Get transactions within a date range
    /// </summary>
    Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateRange dateRange);

    /// <summary>
    /// Get transactions for a specific account within a date range
    /// </summary>
    Task<IEnumerable<Transaction>> GetByAccountAndDateRangeAsync(
        Guid accountId,
        DateRange dateRange);

    /// <summary>
    /// Create a new transaction
    /// </summary>
    Task<Transaction> CreateAsync(Transaction transaction);

    /// <summary>
    /// Update an existing transaction
    /// </summary>
    Task UpdateAsync(Transaction transaction);

    /// <summary>
    /// Get total count of transactions for an account
    /// </summary>
    Task<int> GetCountByAccountIdAsync(Guid accountId);
}