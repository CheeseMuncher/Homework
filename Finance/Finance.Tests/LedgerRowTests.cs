using System;
using Finance.Domain.GoogleSheets.Models;
using FluentAssertions;
using Xunit;

namespace Finance.Tests;

public class LedgerRowTests : TestFixture<LedgerRow>
{
    [Fact]
    public void ToSpreadsheetRow_ReturnsDate_FormattedCorrectly()
    {
        // Arrange
        var row = new LedgerRowBuilder().WithDate(new DateOnly(2022, 03, 04)).Build();

        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        result[0].Should().Be("2022-03-04");
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
        var currency = Create<string>();
        var portfolio = Create<string>();
        var code = Create<string>();
        var row = new LedgerRowBuilder()
            .WithCurrency(currency)
            .WithPortfolio(portfolio)
            .WithCode(code)            
            .Build();

        // Act
        var result = row.ToSpreadsheetRow;

        // Assert
        result[1].Should().Be(currency);
        result[2].Should().Be(portfolio);
        result[3].Should().Be(code);
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

public class LedgerRowBuilder
{
    public DateOnly Date { get; private set; } = new DateOnly(2022,12,30);
    public string Currency { get; private set; } = "GBP";
    public string Portfolio { get; private set; } = "ISA";
    public string Code { get; private set; } = "TPK";
    public decimal Consideration { get; private set; } = 123.456m;
    public int Units { get; private set; } = 78;
    public decimal Price { get; private set; } = 456.789m;
    public decimal LocalExposure { get; private set; } = 123.456m;
    public decimal PositionExposure { get; private set; } = 123.456m;
    public decimal PortfolioExposure { get; private set; } = 123.456m;

    public LedgerRowBuilder WithDate(DateOnly date)
    {
        Date = date;
        return this;
    }
    public LedgerRowBuilder WithCurrency(string currency)
    {
        Currency = currency;
        return this;
    }
    public LedgerRowBuilder WithPortfolio(string portfolio)
    {
        Portfolio = portfolio;
        return this;
    }
    public LedgerRowBuilder WithCode(string code)
    {
        Code = code;
        return this;
    }
    public LedgerRowBuilder WithConsideration(decimal consideration)
    {
        Consideration = consideration;
        return this;
    }
    public LedgerRowBuilder WithUnits(int units)
    {
        Units = units;
        return this;
    }
    public LedgerRowBuilder WithPrice(decimal price)
    {
        Price = price;
        return this;
    }
    public LedgerRowBuilder WithExposures(decimal local, decimal position,decimal portfolio)
    {
        LocalExposure = local;
        PositionExposure = position;
        PortfolioExposure = portfolio;
        return this;
    }

    public LedgerRow Build() => new LedgerRow(Date, Currency, Portfolio, Code, Consideration, Units)
    {
        Price = Price,
        LocalExposure = LocalExposure,
        PositionExposure = PositionExposure,
        PortfolioExposure = PortfolioExposure
    };
}