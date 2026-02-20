using Faraday.Domain.Enums;

namespace Faraday.UI.Models;

public class TransactionDisplay {
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}