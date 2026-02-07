// src/Faraday.Domain/Exceptions/CurrencyMismatchException.cs

using Faraday.Domain.Enums;

namespace Faraday.Domain.Exceptions;

public class CurrencyMismatchException : DomainException {
    public CurrencyType ExpectedCurrency { get; }
    public CurrencyType ActualCurrency { get; }

    public CurrencyMismatchException(CurrencyType expected, CurrencyType actual) 
        : base($"Currency mismatch: expected {expected}, got {actual}") {
        ExpectedCurrency = expected;
        ActualCurrency = actual;
    }
}