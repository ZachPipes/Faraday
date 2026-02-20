// src/Faraday.Domain/Entities/Account.cs

using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;
using Faraday.Domain.Common;
using Faraday.Domain.Enums;
using Faraday.Domain.Exceptions;

namespace Faraday.Domain.Entities;

public class Account : BaseEntity {
    // ================== //
    // Constant Variables //
    // ================== //
    private const int NameLengthLimit = 100;


    // ========= //
    // Variables //
    // ========= //
    public string Name { get; set; }
    public AccountType Type { get; set; }
    public decimal OpeningBalance { get; private set; }
    public CurrencyType Currency { get; set; }
    public bool IsActive { get; private set; } = true;
    public decimal CurrentBalance { get; set; }
    public InstitutionType Institution { get; set; }


    // ================== //
    // Public Constructor //
    // ================== //
    public Account(string name, AccountType type, decimal openingBalance, CurrencyType currency = CurrencyType.Usd,
        InstitutionType institution = InstitutionType.Cash) {
        if (name.Length > NameLengthLimit)
            throw new ArgumentException($"Name cannot be more than {NameLengthLimit} characters");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Account name cannot be empty");

        Name = name;
        Type = type;
        Currency = currency;
        OpeningBalance = openingBalance;
        Institution = institution;
    }


    // ========= //
    // Functions //
    // ========= //
    /// <summary>
    /// Calculates the current balance of all transactions
    /// </summary>
    /// <param name="transactions">All transactions</param>
    /// <returns>The sum of all transactions passed</returns>
    public decimal CalculateCurrentBalance(IEnumerable<Transaction> transactions) {
        ArgumentNullException.ThrowIfNull(transactions);

        // Convert to list to reduce db calls
        List<Transaction> transactionList = transactions.ToList();

        List<Transaction> invalidTransactions = transactionList.Where(t => t.AccountId != Id).ToList();
        if (invalidTransactions.Count != 0)
            throw new InvalidAccountOperationException(
                Id,
                $"Found {invalidTransactions.Count} transactions not belonging to this account");

        decimal transactionSum = transactionList.Where(t => !t.IsVoid).Sum(t => t.Amount);

        CurrentBalance = OpeningBalance + transactionSum;

        return CurrentBalance;
    }

    public IEnumerable<(Transaction Transaction, decimal RunningBalance)> GetTransactionRunningBalances(
        IEnumerable<Transaction> transactions) {
        decimal runningBalance = OpeningBalance;

        foreach (Transaction t in transactions.OrderBy(t => t.Date)) {
            if (!t.IsVoid)
                runningBalance += t.Amount;

            yield return (t, runningBalance);
        }
    }


    /// <summary>
    /// Update the account name
    /// </summary>
    /// <param name="newName">The new account name</param>
    /// <exception cref="ArgumentException">If the new name is null, empty, or more than 100 characters</exception>
    public void UpdateName(string newName) {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Account name cannot be empty", nameof(newName));

        if (newName.Length > NameLengthLimit)
            throw new ArgumentException($"Account name cannot exceed {NameLengthLimit} characters", nameof(newName));

        Name = newName;
        MarkAsModified();
    }

    /// <summary>
    /// Deactivate this account (soft delete)
    /// </summary>
    public void Deactivate() {
        if (!IsActive)
            throw new InvalidAccountOperationException(Id, "Account is already inactive");

        IsActive = false;
        MarkAsModified();
    }

    /// <summary>
    /// Reactivate this account
    /// </summary>
    public void Activate() {
        if (IsActive)
            throw new InvalidAccountOperationException(Id, "Account is already active");

        IsActive = true;
        MarkAsModified();
    }

    /// <summary>
    /// Validate if a transaction can be added to this account
    /// </summary>
    /// <param name="transaction">The transaction to verify</param>
    /// <exception cref="InvalidTransactionException">If the transaction does not belong to the account</exception>
    /// <exception cref="InvalidAccountOperationException">If the account is inactive</exception>
    public void ValidateTransaction(Transaction transaction) {
        if (transaction.AccountId != Id)
            throw new InvalidTransactionException(
                "Transaction does not belong to this account");

        if (!IsActive)
            throw new InvalidAccountOperationException(
                Id,
                "Cannot add transactions to an inactive account");
    }
}