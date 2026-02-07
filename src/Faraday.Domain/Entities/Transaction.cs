// src/Faraday.Domain/Transaction.cs

using Faraday.Domain.Common;
using Faraday.Domain.Enums;
using Faraday.Domain.Exceptions;

namespace Faraday.Domain.Entities;

public class Transaction : BaseEntity {
    // ==================
    // Constant Variables
    // ==================
    private const int DescriptionLengthLimit = 100;
    
    // ===============
    // Class Variables
    // ===============
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public Guid AccountId { get; set; }
    // TODO - Implement categories and transfers
    // public Guid? CategoryId { get; private set; }
    // public Guid? TransferAccountId { get; private set; }
    public bool IsVoid { get; private set; }

    // ==================
    // Public Constructor
    // ==================
    public Transaction(DateTime date, decimal amount, string description, Guid accountId) {
        ValidateCommonFields(date, description);

        Date = date;
        Amount = amount;
        Description = description;
        AccountId = accountId;
        Type = amount >= 0 ? TransactionType.Income : TransactionType.Expense;
    }
    
    /// <summary>
    /// Check if this is an income transaction
    /// </summary>
    public bool IsIncome() => Type == TransactionType.Income && Amount > 0;

    /// <summary>
    /// Check if this is an expense transaction
    /// </summary>
    public bool IsExpense() => Type == TransactionType.Expense && Amount < 0;

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
    
    public void UpdateDescription(string description) {
        if (IsVoid)
            throw new InvalidTransactionException("Cannot update a voided transaction");
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentNullException(nameof(description), "Description cannot be empty");
        
        Description = description;
        MarkAsModified();
    }

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