// tests/Faraday.Domain.Tests/ValueObjects/DateRangeTests.cs

using Faraday.Domain.ValueObjects;

namespace Faraday.Domain.Tests.ValueObjects;

public class DateRangeTests {
    [Fact]
    public void Constructor_WithValidDates_CreatesDateRange() {
        // Arrange
        DateTime start = new(2024, 1, 1);
        DateTime end = new(2024, 1, 31);

        // Act
        DateRange range = new DateRange(start, end);

        // Assert
        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
    }

    [Fact]
    public void Constructor_WithEndBeforeStart_ThrowsException() {
        // Arrange
        DateTime start = new(2024, 1, 31);
        DateTime end = new(2024, 1, 1);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new DateRange(start, end));
    }

    [Fact]
    public void Contains_WithDateInRange_ReturnsTrue() {
        // Arrange
        DateRange range = new(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 31));
        DateTime date = new DateTime(2024, 1, 15);

        // Act & Assert
        Assert.True(range.Contains(date));
    }

    [Fact]
    public void Contains_WithDateOutsideRange_ReturnsFalse() {
        // Arrange
        DateRange range = new(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 31));
        DateTime date = new DateTime(2024, 2, 1);

        // Act & Assert
        Assert.False(range.Contains(date));
    }

    [Fact]
    public void DurationInDays_ReturnsCorrectDuration() {
        // Arrange
        DateRange range = new(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 1, 11)); // 10 days

        // Act
        int duration = range.DurationInDays();

        // Assert
        Assert.Equal(10, duration);
    }

    [Fact]
    public void ForMonth_CreatesCorrectRange() {
        // Act
        DateRange range = DateRange.ForMonth(2024, 2); // February

        // Assert
        Assert.Equal(new DateTime(2024, 2, 1), range.Start);
        Assert.Equal(new DateTime(2024, 3, 1), range.End);
    }

    [Fact]
    public void CurrentMonth_CreatesRangeForCurrentMonth() {
        // Arrange
        DateTime now = DateTime.UtcNow;
        DateTime expectedStart = new(now.Year, now.Month, 1);
        DateTime expectedEnd = expectedStart.AddMonths(1);

        // Act
        DateRange range = DateRange.CurrentMonth();

        // Assert
        Assert.Equal(expectedStart, range.Start);
        Assert.Equal(expectedEnd, range.End);
    }
}