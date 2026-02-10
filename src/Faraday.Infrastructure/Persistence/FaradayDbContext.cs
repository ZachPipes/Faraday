// src/Faraday.Infrastructure/Persistence/FaradayDbContext.cs

using Faraday.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faraday.Infrastructure.Persistence;

/// <summary>
/// EF Core Database Context for Faraday Finance Manager
/// </summary>
public class FaradayDbContext : DbContext {
    public FaradayDbContext(DbContextOptions<FaradayDbContext> options)
        : base(options) { }

    // DbSets (tables)
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FaradayDbContext).Assembly);
    }
}