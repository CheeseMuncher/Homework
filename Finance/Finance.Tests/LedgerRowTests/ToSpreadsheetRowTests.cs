using Finance.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Finance.Tests.LedgerRowTests;

public class ToSpreadsheetRowTests : TestFixture<LedgerRow>
{
    [Fact]
    public void ToSpreadsheetRow_ReturnsDate_FormattedCorrectly()
    {
        // Arrange
        var row = new LedgerRowBuilder().Build();

        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        result[0].Should().Be(row.Date.ToString("yyyy-MM-dd"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public void ToSpreadsheetRow_HandlesNullOrEmptyStrings(string value)
    {
        // Arrange
        var row = new LedgerRowBuilder()
            .WithCurrency(value)
            .WithPortfolio(value)
            .WithCode(value)            
            .Build();
                
        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        result[1].Should().Be("");
        result[2].Should().Be("");
        result[3].Should().Be("");
    }

    [Fact]
    public void ToSpreadsheetRow_MapsStringValues()
    {
        // Arrange
        var row = new LedgerRowBuilder().Build();

        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        result[1].Should().Be(row.Currency);
        result[2].Should().Be(row.Portfolio);
        result[3].Should().Be(row.Code);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ToSpreadsheetRow_MapsConsiderationCorrectly(int sign)
    {
        // Arrange
        var row = new LedgerRowBuilder().WithConsideration((decimal)sign * 123.456m).Build();

        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        result[4].Should().Be($"{(sign==1?"":"-")}123.46");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void ToSpreadsheetRow_MapsUnitsCorrectly(int sign)
    {
        // Arrange
        var row = new LedgerRowBuilder().WithUnits(sign * 789).Build();

        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        result[5].Should().Be($"{(sign==1?"":"-")}789");
    }

    [Fact]
    public void ToSpreadsheetRow_MapsPriceCorrectly()
    {
        // Arrange
        var value = 23m/17m;
        var row = new LedgerRowBuilder().WithPrice(value).Build();

        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        result[6].Should().Be("1.353");
    }

    [Fact]
    public void ToSpreadsheetRow_MapsExposuresCorrectly()
    {
        // Arrange
        var value = 23m/17m;
        var row = new LedgerRowBuilder().WithExposures(value, value * 2, value * 3).Build();
                
        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        result[7].Should().Be("1.35");
        result[8].Should().Be("2.71");
        result[9].Should().Be("4.06");
    }

    [Fact]
    public void ToSpreadsheetRow_MapsZeroValuesCorrectly()
    {
        // Arrange
        var row = new LedgerRowBuilder()
            .WithConsideration(0m)
            .WithUnits(0)
            .WithPrice(0m)
            .WithExposures(0m, 0m, 0m)
            .Build();
                
        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        for (int i = 4; i < 10; i++ )
            result[i].Should().Be("");
    }
}