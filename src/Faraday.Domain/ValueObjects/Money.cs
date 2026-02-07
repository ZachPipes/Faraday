// src/Faraday.Domain/ValueObjects/Money.cs

using Faraday.Domain.Enums;
using Faraday.Domain.Exceptions;

namespace Faraday.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount
/// </summary>
/// <param name="Amount">The amount of money</param>
/// <param name="Currency">The currency of the money</param>
public record Money(decimal Amount, CurrencyType Currency = CurrencyType.Usd) {
    /// <summary>
    /// Add two money values
    /// </summary>
    /// <param name="other">The other money value to add</param>
    /// <returns>A new money object</returns>
    /// <exception cref="InvalidOperationException">If the currencies are not equal, we cannot add them</exception>
    public Money Add(Money other) {
        if (Currency != other.Currency) 
            throw new CurrencyMismatchException(Currency, other.Currency);
        
        return new Money(Amount + other.Amount);
    }

    /// <summary>
    /// Subtract to money values
    /// </summary>
    /// <param name="other">The money value to subtract by</param>
    /// <returns>A new money object</returns>
    /// <exception cref="InvalidOperationException">If the currencies are not equal, we cannot subtract them</exception>
    public Money Subtract(Money other) {
        if (Currency != other.Currency)
            throw new CurrencyMismatchException(Currency, other.Currency);
        
        return new Money(Amount - other.Amount);
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}