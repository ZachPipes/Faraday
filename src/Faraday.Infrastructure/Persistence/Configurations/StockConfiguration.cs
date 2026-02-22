using Faraday.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faraday.Infrastructure.Persistence.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock> {
    public void Configure(EntityTypeBuilder<Stock> builder) {
        // Table name
        builder.ToTable("stocks");

        // Primary key
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Properties
        builder.Property(s => s.RunDate)
            .HasColumnName("run_date")
            .IsRequired();

        builder.Property(s => s.Action)
            .HasColumnName("action")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Symbol)
            .HasColumnName("symbol")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.Commission)
            .HasColumnName("commission")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.Fees)
            .HasColumnName("fees")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.AccruedInterest)
            .HasColumnName("accrued_interest")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.CashBalance)
            .HasColumnName("cash_balance")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(s => s.SettlementDate)
            .HasColumnName("settlement_date");

        builder.Property(s => s.AccountId)
            .HasColumnName("account_id")
            .IsRequired();

        builder.Property(s => s.IsVoid)
            .HasColumnName("is_void")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.ModifiedAt)
            .HasColumnName("modified_at")
            .IsRequired();

        // Foreign key relationship to Account
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(s => s.AccountId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_stocks_account_id");

        // Indexes for better query performance
        builder.HasIndex(s => s.AccountId)
            .HasDatabaseName("ix_stocks_account_id");

        builder.HasIndex(s => s.RunDate)
            .HasDatabaseName("ix_stocks_run_date");

        builder.HasIndex(s => s.IsVoid)
            .HasDatabaseName("ix_stocks_is_void");

        builder.HasIndex(s => new { s.AccountId, s.RunDate })
            .HasDatabaseName("ix_stocks_account_run_date");
    }
}