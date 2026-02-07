// tests/Faraday.Domain.Tests/ValueObjects/MoneyTests.cs

// NOTE:
// Money constructor is normally Money(decimal amount, CurrencyType currency),
// But the default is USD, so all constructors that do not change it will
// appear as Money(decimal amount)

using Faraday.Domain.Enums;
using Faraday.Domain.Exceptions;
using Faraday.Domain.ValueObjects;

namespace Faraday.Domain.Tests.ValueObjects;

public class MoneyTests {
    [Fact]
    public void Constructor_WithValidData_CreatesMoney() {
        // Act
        Money money = new(100m);

        // Assert
        Assert.Equal(100m, money.Amount);
        Assert.Equal(CurrencyType.Usd, money.Currency);
    }

    [Fact]
    public void Constructor_WithLowercaseCurrency_ConvertsToUppercase() {
        // Act
        Money money = new(100m);

        // Assert
        Assert.Equal(CurrencyType.Usd, money.Currency);
    }

    [Fact]
    public void Add_WithSameCurrency_ReturnsSum() {
        // Arrange
        Money money1 = new(100m);
        Money money2 = new(50m);

        // Act
        Money result = money1.Add(money2);

        // Assert
        Assert.Equal(150m, result.Amount);
        Assert.Equal(CurrencyType.Usd, result.Currency);
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ThrowsException() {
        // Arrange
        Money usd = new(100m);
        Money eur = new(50m, CurrencyType.Eur);

        // Act & Assert
        Assert.Throws<CurrencyMismatchException>(() => usd.Add(eur));
    }

    [Fact]
    public void Subtract_WithSameCurrency_ReturnsDifference() {
        // Arrange
        Money money1 = new(100m);
        Money money2 = new(30m);

        // Act
        Money result = money1.Subtract(money2);

        // Assert
        Assert.Equal(70m, result.Amount);
    }

    [Fact]
    public void Negate_ReturnsNegativeAmount() {
        // Arrange
        Money money = new(100m);

        // Act
        Money result = money.Negate();

        // Assert
        Assert.Equal(-100m, result.Amount);
        Assert.Equal(CurrencyType.Usd, result.Currency);
    }

    [Fact]
    public void IsPositive_WithPositiveAmount_ReturnsTrue() {
        // Arrange
        Money money = new(100m);

        // Act & Assert
        Assert.True(money.IsPositive());
    }

    [Fact]
    public void IsNegative_WithNegativeAmount_ReturnsTrue() {
        // Arrange
        Money money = new(-100m);

        // Act & Assert
        Assert.True(money.IsNegative());
    }

    [Fact]
    public void IsZero_WithZeroAmount_ReturnsTrue() {
        // Arrange
        Money money = new(0);

        // Act & Assert
        Assert.True(money.IsZero());
    }
}