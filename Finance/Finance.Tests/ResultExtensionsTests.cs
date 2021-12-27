using Finance.Domain.Yahoo;
using Finance.Utils;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Finance.Tests;

public class ResultExtensionsTests : TestFixture
{
    [Fact]
    public void ToPriceSet_CreatesKeyValuePairForSuppliedDate_IfDateNotInResult()
    {
        // Arrange
        var newDate = Create<DateTime>();
        var apiResult = Create<Result>();
        apiResult.meta["symbol"] = Create<string>();

        // Act
        var result = apiResult.ToPriceSet(new [] { newDate });

        // Assert
        result.Prices.Keys.Should().Contain(newDate);
        result.Prices[newDate].Should().BeEmpty();
    }

    [Fact]
    public void ToPriceSet_CreatesKeyValuePairForResultDate_IfDateNotInSuppliedSet()
    {
        // Arrange
        var apiResult = Create<Result>();
        apiResult.meta["symbol"] = Create<string>();
        for(int i = 0; i < apiResult.timestamp.Count(); i++)
            apiResult.timestamp[i] *= 1000000;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>());

        // Assert
        foreach (var stamp in apiResult.timestamp)
            result.Prices.Keys.Should().Contain(stamp.UnixToDateTime().Date);
    }

    [Fact]
    public void ToPriceSet_AddsStockPricesFromResult()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<Result>();
        apiResult.meta["symbol"] = stock;
        for(int i = 0; i < apiResult.timestamp.Count(); i++)
            apiResult.timestamp[i] *= 1000000;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>());

        // Assert
        foreach (var stamp in apiResult.timestamp)
        {
            result.Prices[stamp.UnixToDateTime().Date].Should().Contain(sp => sp.Stock == stock);
        }
    }

    [Fact]
    public void ToPriceSet_TakesClosingPrice()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<Result>();
        apiResult.timestamp = new [] { apiResult.timestamp.First() };
        apiResult.indicators.quote = new [] { apiResult.indicators.quote.First() };
        apiResult.meta["symbol"] = stock;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>());

        // Assert
        var expectedPrice = apiResult.indicators.quote.Single().close[0];
        result.Prices.Values.Single().Single().Price.Should().Be(expectedPrice);
    }

    [Fact]
    public void ToPriceSet_RoundsClosingPrice()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<Result>();
        apiResult.timestamp = new [] { apiResult.timestamp.First() };
        apiResult.indicators.quote = new [] { apiResult.indicators.quote.First() };
        apiResult.meta["symbol"] = stock;
        apiResult.indicators.quote.Single().close[0] = 0.123456789m;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>());

        // Assert        
        result.Prices.Values.Single().Single().Price.Should().Be(0.123457m);
    }   
}

