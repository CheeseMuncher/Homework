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
        var apiResult = Create<Response>();

        // Act
        var result = apiResult.ToPriceSet(new [] { newDate }, Create<string>());

        // Assert
        result.Prices.Keys.Should().Contain(newDate);
        result.Prices[newDate].Should().BeEmpty();
    }

    [Fact]
    public void ToPriceSet_CreatesKeyValuePairForResultDate_IfDateNotInSuppliedSet()
    {
        // Arrange
        var apiResult = Create<Response>();
        foreach(var price in apiResult.prices)
            price.date *= 1000000;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>(), Create<string>());

        // Assert
        foreach(var price in apiResult.prices)
            result.Prices.Keys.Should().Contain(price.date.UnixToDateTime().Date);
    }

    [Fact]
    public void ToPriceSet_AddsStockPricesFromResult()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<Response>();
        foreach(var price in apiResult.prices)
            price.date *= 1000000;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>(), stock);

        // Assert
        foreach(var price in apiResult.prices)
            result.Prices[price.date.UnixToDateTime().Date].Should().Contain(sp => sp.Stock == stock);
    }

    [Fact]
    public void ToPriceSet_TakesClosingPrice()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<Response>();
        apiResult.prices = new [] { apiResult.prices.First() };

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>(), stock);

        // Assert
        result.Prices.Values.Single().Single().Price.Should().Be(apiResult.prices.Single().close);
    }

    [Fact]
    public void ToPriceSet_RoundsClosingPrice()
    {
        // Arrange
        var stock = Create<string>();
        var apiResult = Create<Response>();
        apiResult.prices = new [] { apiResult.prices.First() };
        apiResult.prices.Single().close = 0.123456789m;

        // Act
        var result = apiResult.ToPriceSet(Array.Empty<DateTime>(), stock);

        // Assert        
        result.Prices.Values.Single().Single().Price.Should().Be(0.123457m);
    }   
}