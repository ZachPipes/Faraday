// src/Faraday.Domain/ValueObjects/DateRange.cs

namespace Faraday.Domain.ValueObjects;

/// <summary>
/// Represents a date range (used for reporting periods, document periods, etc
/// </summary>
public record DateRange {
    public DateTime Start { get; init; }
    public DateTime End { get; init; }

    public DateRange(DateTime start, DateTime end) {
        if (end < start) {
            throw new ArgumentOutOfRangeException($"End date ({end}) must be greater than start date ({start})");
        }
        
        Start = start;
        End = end;
    }

    /// <summary>
    /// Create a date range for a specific month
    /// </summary>
    /// <param name="year">The specific year</param>
    /// <param name="month">The specific month</param>
    /// <returns>A new DateRange object</returns>
    public static DateRange ForMonth(int year, int month) {
        DateTime start = new(year, month, 1);
        DateTime end = start.AddMonths(1);
        return new DateRange(start, end);
    }

    /// <summary>
    /// Create a date range for the current month
    /// </summary>
    /// <returns>A DateRange for the current month</returns>
    public static DateRange CurrentMonth() {
        DateTime now = DateTime.UtcNow;
        return ForMonth(now.Year, now.Month);
    }
    
    /// <summary>
    /// Check if a date falls within this range
    /// </summary>
    /// <param name="date">The specified date</param>
    /// <returns>True if the date is within the range, false elsewise</returns>
    public bool Contains(DateTime date) => (date >= Start && date <= End);
    
    /// <summary>
    /// Get the number of days in this range
    /// </summary>
    /// <returns>The number of days in this range</returns>
    public int DurationInDays() => (End - Start).Days;

    public override string ToString() => $"{Start:yyyy-MM-dd} to {End:yyyy-MM-dd}";
}