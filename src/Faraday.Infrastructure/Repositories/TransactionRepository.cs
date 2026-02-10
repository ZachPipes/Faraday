// src/Faraday.Infrastructure/Repositories/TransactionRepository.cs

using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;
using Faraday.Domain.ValueObjects;
using Faraday.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Faraday.Infrastructure.Repositories;

/// <summary>
/// Sqlite implementation of ITransactionRepository
/// </summary>
public class TransactionRepository : ITransactionRepository {
    private readonly FaradayDbContext _context;

    public TransactionRepository(FaradayDbContext context) {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id) {
        return await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId) {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetActiveByAccountIdAsync(Guid accountId) {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .Where(t => !t.IsVoid)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateRange dateRange) {
        return await _context.Transactions
            .Where(t => t.Date >= dateRange.Start && t.Date < dateRange.End)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByAccountAndDateRangeAsync(Guid accountId, DateRange dateRange) {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .Where(t => t.Date >= dateRange.Start && t.Date < dateRange.End)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<Transaction> CreateAsync(Transaction transaction) {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task UpdateAsync(Transaction transaction) {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetCountByAccountIdAsync(Guid accountId) {
        return await _context.Transactions
            .Where(t => t.AccountId == accountId)
            .Where(t => !t.IsVoid)
            .CountAsync();
    }
}