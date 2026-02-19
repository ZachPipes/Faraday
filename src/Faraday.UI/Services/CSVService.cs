using System.IO;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.UI.Services;

public class CSVService : ICSVService {
    public IEnumerable<T> Parse<T>(string filePath, Guid accountId) where T : class {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CSV file not found: {filePath}");

        IEnumerable<string> lines = File.ReadAllLines(filePath).Skip(1);

        foreach (string line in lines) {
            string[] parts = line.Split(',');

            if (typeof(T) == typeof(Transaction)) {
                yield return new Transaction(
                    date: DateTime.Parse(parts[0].Trim('"')),
                    no: BasicUtilities.GetNonNullValue<int>(parts[1].Trim('"')),
                    description: parts[2].Trim('"'),
                    amount: -(BasicUtilities.GetNonNullValue<decimal>(parts[3].Trim('"'))) + BasicUtilities.GetNonNullValue<decimal>(parts[4].Trim('"')),
                    accountId: accountId
                ) as T ?? throw new InvalidOperationException($"Transaction could not be parsed Date: {DateTime.Parse(parts[0].Trim('"'))} " +
                                                              $"| Amount: {-(BasicUtilities.GetNonNullValue<decimal>(parts[3].Trim('"')) + 
                                                                             BasicUtilities.GetNonNullValue<decimal>(parts[4].Trim('"')))}");
            }
            // TODO - Implement Stock transaction reading
            // else if (typeof(T) == typeof(StockTransaction))
            // {
            //     yield return new StockTransaction
            //     {
            //         Date = DateTime.Parse(parts[0].Trim('"')),
            //         Symbol = parts[1].Trim('"'),
            //         TransactionType = parts[2].Trim('"'), // e.g., "Buy", "Sell"
            //         Quantity = BasicUtilities.GetNullableValue<decimal>(parts[3].Trim('"')),
            //         Price = BasicUtilities.GetNullableValue<decimal>(parts[4].Trim('"')),
            //         Fees = BasicUtilities.GetNullableValue<decimal>(parts[5].Trim('"'))
            //     } as T;
            // }
            else {
                throw new NotSupportedException($"CSV parsing for {typeof(T).Name} is not implemented.");
            }
        }
    }
}