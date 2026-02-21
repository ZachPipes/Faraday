// src/Faraday.Application/Interfaces/IStockRepository.cs

using Faraday.Domain.Entities;
using Faraday.Domain.ValueObjects;

namespace Faraday.Application.Interfaces;

/// <summary>
/// Repository interface for Stock persistence operations
/// </summary>
public interface IStockRepository {
    /// <summary>
    /// Get a Stock by its ID
    /// </summary>
    Task<Stock?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all Stocks for a specific account
    /// </summary>
    Task<IEnumerable<Stock>> GetByAccountIdAsync(Guid accountId);

    /// <summary>
    /// Get all non-voided Stocks for a specific account
    /// </summary>
    Task<IEnumerable<Stock>> GetActiveByAccountIdAsync(Guid accountId);

    /// <summary>
    /// Get Stocks within a date range
    /// </summary>
    Task<IEnumerable<Stock>> GetByDateRangeAsync(DateRange dateRange);

    /// <summary>
    /// Get Stocks for a specific account within a date range
    /// </summary>
    Task<IEnumerable<Stock>> GetByAccountAndDateRangeAsync(
        Guid accountId,
        DateRange dateRange);

    /// <summary>
    /// Create a new Stock
    /// </summary>
    Task<Stock> CreateAsync(Stock Stock);

    /// <summary>
    /// Update an existing Stock
    /// </summary>
    Task UpdateAsync(Stock Stock);

    /// <summary>
    /// Get total count of Stocks for an account
    /// </summary>
    Task<int> GetCountByAccountIdAsync(Guid accountId);
}