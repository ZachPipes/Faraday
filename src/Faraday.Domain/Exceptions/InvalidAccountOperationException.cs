// src/Faraday.Domain/Exceptions/InvalidAccountOperationException.cs

namespace Faraday.Domain.Exceptions;

public class InvalidAccountOperationException : DomainException {
    public Guid AccountId { get; }

    public InvalidAccountOperationException(Guid accountId, string message) : base(message) {
        AccountId = accountId;
    }
}