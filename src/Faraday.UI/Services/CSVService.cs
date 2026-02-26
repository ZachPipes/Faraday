using System.IO;
using Faraday.Application.Interfaces;
using Faraday.Domain.Entities;

namespace Faraday.UI.Services;

public class CSVService : ICSVService {
    public async IAsyncEnumerable<T> Parse<T>(string filePath, Guid accountId) where T : class {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CSV file not found: {filePath}");

        IEnumerable<string> lines = (await File.ReadAllLinesAsync(filePath)).Where(line => !string.IsNullOrWhiteSpace(line));

        bool started = false;
        foreach (string line in lines) {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(',');

            // If first column isn't a date:
            if (!DateTime.TryParse(parts[0].Trim('"'), out DateTime parsedDate)) {
                // If we haven't started yet, still in header
                if (!started)
                    continue;

                // If we already started, we've reached metadata/footer
                yield break;
            }

            started = true;

            if (typeof(T) == typeof(Transaction)) {
                yield return new Transaction(
                    parsedDate,
                    no: BasicUtilities.GetNonNullValue<int>(parts[1].Trim('"')),
                    description: parts[2].Trim('"'),
                    amount: -(BasicUtilities.GetNonNullValue<decimal>(parts[3].Trim('"'))) +
                            BasicUtilities.GetNonNullValue<decimal>(parts[4].Trim('"')),
                    accountId: accountId
                ) as T ?? throw new InvalidOperationException(
                    $"Transaction could not be parsed Date: {DateTime.Parse(parts[0].Trim('"'))} " +
                    $"| Amount: {-(BasicUtilities.GetNonNullValue<decimal>(parts[3].Trim('"')) +
                                   BasicUtilities.GetNonNullValue<decimal>(parts[4].Trim('"')))}");
            }
            else if (typeof(T) == typeof(Stock)) {
                DateTime? settlementDate = null;

                string rawSettlement = parts[12].Trim('"');

                if (!string.IsNullOrWhiteSpace(rawSettlement) &&
                    DateTime.TryParse(rawSettlement, out DateTime parsedSettlement)) {
                    settlementDate = parsedSettlement;
                }

                yield return new Stock(
                    parsedDate,
                    parts[1].Trim('"'),
                    parts[2].Trim('"'),
                    parts[3].Trim('"'),
                    parts[4].Trim('"'),
                    BasicUtilities.GetNonNullValue<decimal>(parts[5].Trim('"')),
                    BasicUtilities.GetNonNullValue<decimal>(parts[6].Trim('"')),
                    BasicUtilities.GetNonNullValue<decimal>(parts[7].Trim('"')),
                    BasicUtilities.GetNonNullValue<decimal>(parts[8].Trim('"')),
                    BasicUtilities.GetNonNullValue<decimal>(parts[9].Trim('"')),
                    BasicUtilities.GetNonNullValue<decimal>(parts[10].Trim('"')),
                    BasicUtilities.GetNonNullValue<decimal>(parts[11].Trim('"')),
                    settlementDate,
                    accountId
                ) as T ?? throw new InvalidOperationException(
                    $"Stock could not be parsed Date: {DateTime.Parse(parts[0].Trim('"'))} " +
                    $"| Amount: {BasicUtilities.GetNonNullValue<decimal>(parts[10].Trim('"'))}");
            }
            else {
                throw new NotSupportedException($"CSV parsing for {typeof(T).Name} is not implemented.");
            }
        }
    }
}