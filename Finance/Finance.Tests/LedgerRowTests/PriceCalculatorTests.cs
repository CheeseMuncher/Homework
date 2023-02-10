using Finance.Domain.GoogleSheets;
using FluentAssertions;
using Xunit;

namespace Finance.Tests.LedgerRowTests;

public class PriceCalculatorTests : TestFixture<PriceCalculator>
{
    [Fact]
    public void CalculatePrice_ReturnsZero_IfUnitsZero()
    {
        // Arrange
        var row = new LedgerRowBuilder().WithUnits(0).Build();
        
        // Act
        var result = Sut.CalculatePrice(row);

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public void CalculatePrice_CalculatesBuyPriceCorrectly()
    {
        // Arrange
        var row = new LedgerRowBuilder()
            .WithConsideration(-8959.85m)
            .WithUnits(490)
            .Build();
        
        // Act
        var result = Sut.CalculatePrice(row);

        // Assert
        result.ToString("0.000").Should().Be("1828.541");
    }

    [Fact]
    public void CalculatePrice_CalculatesSellPriceCorrectly()
    {
        // Arrange
        var row = new LedgerRowBuilder()
            .WithConsideration(15617.97m)
            .WithUnits(-490)
            .Build();
        
        // Act
        var result = Sut.CalculatePrice(row);

        // Assert
        result.ToString("0.000").Should().Be("3187.341");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void CalculatePrice_IgnoresConsiderationBelow2k(int sign)
    {
        // Arrange
        var row = new LedgerRowBuilder()
            .WithConsideration(sign * 1999.99m)
            .WithUnits(-1 * sign * 100)
            .Build();
        
        // Act
        var result = Sut.CalculatePrice(row);

        // Assert
        result.Should().Be(0);
    }
}