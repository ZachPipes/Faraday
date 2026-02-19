// tests/Faraday.Infrastructure.Tests/SqLiteConnectionTests.cs

using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Faraday.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Faraday.Infrastructure.Tests;

public class SqLiteConnectionTests : IDisposable {
    private readonly FaradayDbContext _context;

    public SqLiteConnectionTests() {
        SqliteConnection connection = new("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FaradayDbContext>()
            .UseSqlite(connection)
            .Options;

        _context = new FaradayDbContext(options);
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CanCreateAndRetrieveAccount() {
        // Arrange
        Account account = new("Test SQLite Account", AccountType.Checking, 1000m);

        // Act - Create
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        // Clear tracking to simulate new context
        _context.ChangeTracker.Clear();

        // Act - Retrieve
        Account? retrieved = await _context.Accounts.FindAsync(account.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Test SQLite Account", retrieved.Name);
        Assert.Equal(1000m, retrieved.OpeningBalance);
    }

    [Fact]
    public async Task CanCreateTransactionWithForeignKey() {
        // Arrange
        Account account = new("Test Account", AccountType.Checking, 1000m);
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        Transaction transaction = new(
            DateTime.UtcNow,
            500m,
            "Test Transaction",
            0,
            account.Id);

        // Act
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();

        // Assert
        Transaction? retrieved = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transaction.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(account.Id, retrieved.AccountId);
    }

    public void Dispose() {
        _context.Dispose();
    }
}