using Finance.Domain.Yahoo;
using Finance.Domain.Yahoo.Models;
using Finance.Utils;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Finance.Tests;

public class ResponseExtensionsTests : TestFixture
{
    [Fact]
    public void HistoryResponseToPriceSet_CreatesKeyValuePairForSuppliedDate_IfDateNotInResult()
    {
        // Arrange
        var newDate = Create<DateTime>();
        var apiResult = Create<HistoryResponse>();

        // Act
        var result = apiResult.ToPriceSet(new [] { newDate }, Create<string>());

        // Assert
        result.Prices.Keys.Should().Contain(newDate);
        result.Prices[newDate].Should().BeEmpty();
    }

    [Fact]
    public void HistoryResponseToPriceSet_CreatesKeyValuePairForResultDate_IfDateNotInSuppliedSet()
    {
        // Arrange
        var apiResult = Create<HistoryResponse>();
        foreach(var price in apiResult.prices)
            price.date *= 1000000;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>(), Create<string>());

        // Assert
        foreach(var price in apiResult.prices)
            result.Prices.Keys.Should().Contain(price.date.UnixToDateTime().Date);
    }

    [Fact]
    public void HistoryResponseToPriceSet_AddsStockPricesFromResult()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<HistoryResponse>();
        foreach(var price in apiResult.prices)
            price.date *= 1000000;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>(), stock);

        // Assert
        foreach(var price in apiResult.prices)
            result.Prices[price.date.UnixToDateTime().Date].Should().Contain(sp => sp.Stock == stock);
    }

    [Fact]
    public void HistoryResponseToPriceSet_DoesNotAddDividendPriceDataFromResult()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<HistoryResponse>();
        apiResult.prices[0].close = 0m;
        foreach(var price in apiResult.prices)
            price.date *= 1000000;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>(), stock);

        // Assert
        result.Prices[apiResult.prices[0].date.UnixToDateTime().Date].Should().NotContain(sp => sp.Stock == stock);
    }

    [Fact]
    public void HistoryResponseToPriceSet_TakesClosingPrice()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<HistoryResponse>();
        apiResult.prices = new [] { apiResult.prices.First() };

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>(), stock);

        // Assert
        result.Prices.Values.Single().Single().Price.Should().Be(apiResult.prices.Single().close);
    }

    [Fact]
    public void HistoryResponseToPriceSet_RoundsClosingPrice()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<HistoryResponse>();
        apiResult.prices = new [] { apiResult.prices.First() };
        apiResult.prices.Single().close = 0.123456789m;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>(), stock);

        // Assert        
        result.Prices.Values.Single().Single().Price.Should().Be(0.123457m);
    }

        [Fact]
    public void ChartResultToPriceSet_CreatesKeyValuePairForSuppliedDate_IfDateNotInResult()
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
    public void ChartResultToPriceSet_CreatesKeyValuePairForResultDate_IfDateNotInSuppliedSet()
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
    public void ChartResultToPriceSet_AddsStockPricesFromResult()
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
    public void ChartResultToPriceSet_TakesClosingPrice()
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
    public void ChartResultToPriceSet_RoundsClosingPrice()
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