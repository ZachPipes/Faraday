// src/Faraday.Application/Interfaces/IAccountRepository.cs

using Faraday.Domain.Entities;

namespace Faraday.Application.Interfaces;

/// <summary>
/// Repository interface for Account persistence operations
/// Infrastructure layer will implement this
/// </summary>
public interface IAccountRepository {
    /// <summary>
    /// Get an account by its ID
    /// </summary>
    Task<Account?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all active accounts
    /// </summary>
    Task<IEnumerable<Account>> GetAllActiveAsync();

    /// <summary>
    /// Get all accounts (including inactive)
    /// </summary>
    Task<IEnumerable<Account>> GetAllAsync();

    /// <summary>
    /// Create a new account
    /// </summary>
    Task<Account> CreateAsync(Account account);

    /// <summary>
    /// Update an existing account
    /// </summary>
    Task UpdateAsync(Account account);

    /// <summary>
    /// Check if an account with the given name already exists
    /// </summary>
    Task<bool> ExistsWithNameAsync(string name);
}