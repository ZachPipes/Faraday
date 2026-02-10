// src/Faraday.Infrastructure/Persistence/Configurations/AccountConfiguration.cs

using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faraday.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Account entity
/// </summary>
public class AccountConfiguration : IEntityTypeConfiguration<Account> {
    public void Configure(EntityTypeBuilder<Account> builder) {
        // Table name
        builder.ToTable("accounts");

        // Primary key
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // GUIDs generated in domain

        // Properties
        builder.Property(a => a.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (AccountType)Enum.Parse(typeof(AccountType), v));

        builder.Property(a => a.Currency)
            .HasColumnName("currency")
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue(CurrencyType.Usd);

        builder.Property(a => a.OpeningBalance)
            .HasColumnName("opening_balance")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.ModifiedAt)
            .HasColumnName("modified_at")
            .IsRequired();

        // Indexes for better query performance
        builder.HasIndex(a => a.Name)
            .HasDatabaseName("ix_accounts_name");

        builder.HasIndex(a => a.Type)
            .HasDatabaseName("ix_accounts_type");

        builder.HasIndex(a => a.IsActive)
            .HasDatabaseName("ix_accounts_is_active");
    }
}