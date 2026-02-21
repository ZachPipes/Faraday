// src/Faraday.Infrastructure/Repositories/StockRepository.cs

using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;
using Faraday.Domain.ValueObjects;
using Faraday.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Faraday.Infrastructure.Repositories;

/// <summary>
/// Sqlite implementation of IStockRepository
/// </summary>
public class StockRepository : IStockRepository {
    private readonly FaradayDbContext _context;

    public StockRepository(FaradayDbContext context) {
        _context = context;
    }
    
    public async Task<Stock?> GetByIdAsync(Guid id) {
        return await _context.Stocks
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Stock>> GetByAccountIdAsync(Guid accountId) {
        return await _context.Stocks
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.RunDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Stock>> GetActiveByAccountIdAsync(Guid accountId) {
        return await _context.Stocks
            .Where(t => t.AccountId == accountId)
            .Where(t => !t.IsVoid)
            .OrderByDescending(t => t.RunDate)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Stock>> GetByDateRangeAsync(DateRange dateRange) {
        return await _context.Stocks
            .Where(t => t.RunDate >= dateRange.Start && t.RunDate < dateRange.End)
            .OrderByDescending(t => t.RunDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Stock>> GetByAccountAndDateRangeAsync(Guid accountId, DateRange dateRange) {
        return await _context.Stocks
            .Where(t => t.AccountId == accountId)
            .Where(t => t.RunDate >= dateRange.Start && t.RunDate < dateRange.End)
            .OrderByDescending(t => t.RunDate)
            .ToListAsync();
    }

    public async Task<Stock> CreateAsync(Stock stock) {
        await _context.Stocks.AddAsync(stock);
        await _context.SaveChangesAsync();
        return stock;
    }

    public async Task UpdateAsync(Stock stock) {
        _context.Stocks.Update(stock);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetCountByAccountIdAsync(Guid accountId) {
        return await _context.Stocks
            .Where(t => t.AccountId == accountId)
            .Where(t => !t.IsVoid)
            .CountAsync();
    }
}