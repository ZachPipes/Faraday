// src/Faraday.Domain/Exceptions/InvalidTransactionException.cs

namespace Faraday.Domain.Exceptions;

public class InvalidTransactionException : DomainException {
    public InvalidTransactionException(string message) : base(message) { }
}