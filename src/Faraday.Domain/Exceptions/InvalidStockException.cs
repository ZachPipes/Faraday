namespace Faraday.Domain.Exceptions;

public class InvalidStockException : DomainException {
    public InvalidStockException(string message) : base(message) { }
}