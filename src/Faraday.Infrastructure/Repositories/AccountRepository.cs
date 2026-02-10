// src/Faraday.Infrastructure/Repositories/AccountRepository.cs

using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;
using Faraday.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Faraday.Infrastructure.Repositories;

/// <summary>
/// PostgreSQL implementation of IAccountRepository
/// </summary>
public class AccountRepository : IAccountRepository {
    private readonly FaradayDbContext _context;

    public AccountRepository(FaradayDbContext context) {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid id) {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Account>> GetAllActiveAsync() {
        return await _context.Accounts
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Account>> GetAllAsync() {
        return await _context.Accounts
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Account> CreateAsync(Account account) {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task UpdateAsync(Account account) {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsWithNameAsync(string name) {
        return await _context.Accounts
            .AnyAsync(a => a.Name == name);
    }
}