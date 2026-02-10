// src/Faraday.Infrastructure/Persistence/FaradayDbContextFactory.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Faraday.Infrastructure.Persistence;

/// <summary>
/// Factory for creating DbContext at design time (for migrations)
/// </summary>
public class FaradayDbContextFactory : IDesignTimeDbContextFactory<FaradayDbContext> {
    public FaradayDbContext CreateDbContext(string[] args) {
        DbContextOptionsBuilder<FaradayDbContext> optionsBuilder = new();
        
        optionsBuilder.UseSqlite("Data Source=faraday.db");

        return new FaradayDbContext(optionsBuilder.Options);
    }
}