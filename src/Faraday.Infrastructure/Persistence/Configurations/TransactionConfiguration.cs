// src/Faraday.Infrastructure/Persistence/Configurations/TransactionConfiguration.cs

using Faraday.Domain.Entities;
using Faraday.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faraday.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Transaction entity
/// </summary>
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction> {
    public void Configure(EntityTypeBuilder<Transaction> builder) {
        // Table name
        builder.ToTable("transactions");

        // Primary key
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Properties
        builder.Property(t => t.Date)
            .HasColumnName("date")
            .IsRequired();

        builder.Property(t => t.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (TransactionType)Enum.Parse(typeof(TransactionType), v));

        builder.Property(t => t.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(t => t.IsVoid)
            .HasColumnName("is_void")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(t => t.ModifiedAt)
            .HasColumnName("modified_at")
            .IsRequired();

        // Foreign key relationship to Account
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_transactions_account_id");

        // Indexes for better query performance
        builder.HasIndex(t => t.AccountId)
            .HasDatabaseName("ix_transactions_account_id");

        builder.HasIndex(t => t.Date)
            .HasDatabaseName("ix_transactions_date");

        builder.HasIndex(t => t.IsVoid)
            .HasDatabaseName("ix_transactions_is_void");

        builder.HasIndex(t => new { t.AccountId, t.Date })
            .HasDatabaseName("ix_transactions_account_date");
    }
}