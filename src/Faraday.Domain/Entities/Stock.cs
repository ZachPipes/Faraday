// src/Faraday.Domain/Stock.cs
using Faraday.Domain.Common;
using Faraday.Domain.Enums;
using Faraday.Domain.Exceptions;

namespace Faraday.Domain.Entities;

public class Stock : BaseEntity {
    // ================== //
    // Constant Variables //
    // ================== //
    private const int DescriptionLengthLimit = 100;


    // ========= //
    // Variables //
    // ========= //
    public DateTime RunDate { get; set; }
    public string Action { get; set; }
    public string Symbol { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public decimal Commission { get; set; }
    public decimal Fees { get; set; }
    public decimal AccruedInterest { get; set; }
    public decimal Amount { get; set; }
    public decimal CashBalance { get; set; }
    public DateTime SettlementDate { get; set; }
    public Guid AccountId { get; set; }
    
    public bool IsVoid { get; private set; }


    // ================== //
    // Public Constructor //
    // ================== //
    public Stock(DateTime runDate, string action, string symbol, string description, string type,
        decimal price, decimal quantity, decimal commission, decimal fees, decimal accruedInterest, 
        decimal amount, decimal cashBalance, DateTime settlementDate, Guid accountId) {
        ValidateCommonFields(runDate, description);

        RunDate = runDate;
        Action = action;
        Symbol = symbol;
        Description = description;
        Type = type;
        Price = price;
        Quantity = quantity;
        Commission = commission;
        Fees = fees;
        AccruedInterest = accruedInterest;
        Amount = amount;
        CashBalance = cashBalance;
        SettlementDate = settlementDate;
        AccountId = accountId;
    }

    /// <summary>
    /// Mark this transaction as void
    /// </summary>
    /// <exception cref="InvalidTransactionException">If the transaction is already void</exception>
    public void Void() {
        if (IsVoid)
            throw new InvalidTransactionException("Transaction is already voided");

        IsVoid = true;
        MarkAsModified();
    }

    /// <summary>
    /// Updates the transaction description with the passed description
    /// </summary>
    /// <param name="description">The new description</param>
    /// <exception cref="InvalidTransactionException">If the transaction is voided</exception>
    /// <exception cref="ArgumentNullException">If the transaction is null/white space</exception>
    public void UpdateDescription(string description) {
        if (IsVoid)
            throw new InvalidTransactionException("Cannot update a voided transaction");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentNullException(nameof(description), "Description cannot be empty");

        Description = description;
        MarkAsModified();
    }

    /// <summary>
    /// Validates the date and description
    /// </summary>
    /// <param name="date">The date to validate</param>
    /// <param name="description">The description to validate</param>
    /// <exception cref="InvalidTransactionException">If date is in the future</exception>
    /// <exception cref="ArgumentException">If description is null/white space or is longer than the DescriptionLengthLimit variable</exception>
    private static void ValidateCommonFields(DateTime date, string description) {
        if (date > DateTime.UtcNow.AddDays(1))
            throw new InvalidTransactionException("Cannot update a transaction with a date in the future");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        if (description.Length > DescriptionLengthLimit)
            throw new ArgumentException(
                $"Description cannot be longer than {DescriptionLengthLimit} characters", nameof(description));
    }
}