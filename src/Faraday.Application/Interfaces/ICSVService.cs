namespace Faraday.Application.Interfaces;

/// <summary>
/// Provides CSV parsing services for arbitrary record types.
/// </summary>
public interface ICSVService {
    /// <summary>
    /// Parses a CSV file into a collection of strongly-typed records.
    /// </summary>
    /// <typeparam name="T">The type of record to parse.</typeparam>
    /// <param name="filePath">The path to the CSV file.</param>
    /// <param name="accountId">The account id to link</param>
    /// <returns>An enumerable of parsed records.</returns>
    IEnumerable<T> Parse<T>(string filePath, Guid accountId) where T : class;
}