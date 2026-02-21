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
        
        string localFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        const string faradayFolderName = "Faraday";
        string faradayFolder = Path.Combine(localFolder, faradayFolderName);
        string dbPath = Path.Combine(faradayFolder, "faraday.sqlite");
        
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new FaradayDbContext(optionsBuilder.Options);
    }
}