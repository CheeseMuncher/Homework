using Finance.Domain.Prices;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Finance.Tests;

public class PriceSetExtensionsTests : TestFixture
{
    [Fact]
    public void AddDates_AddsNewKeyValuePairs_IfNewDatesSupplied()
    {
        // Arrange
        var newDates = Create<DateTime[]>();

        // Act
        var result = new PriceSet().AddDates(newDates);

        // Assert
        result.Prices.Keys.Should().BeEquivalentTo(newDates);
        result.Prices.Values.Should().AllBeEquivalentTo(new HashSet<StockPrice>());
    }

    [Fact]
    public void AddDates_DoesNotAddNewKeyValuePair_IfDateKeyAlreadyExists()
    {
        // Arrange
        var prices = Create<PriceSet>();
        var newDate = prices.Prices.Keys.First();

        // Act
        var result = prices.AddDates(new [] { newDate });

        // Assert
        result.Prices[newDate].Should().NotBeEmpty();
    }

    [Fact]
    public void AddPrices_AddsNewKeyValuePairs_IfNewPriceSetContainsNewDates()
    {
        // Arrange
        var newPrices = Create<PriceSet>();

        // Act
        var result = new PriceSet().AddPrices(newPrices, Create<string>());

        // Assert
        result.Prices.Keys.Should().BeEquivalentTo(newPrices.Prices.Keys);
    }

    [Fact]
    public void AddPrices_AddsStockPriceFromNewSetToExistingForMatchingDate()
    {
        // Arrange
        var prices = Create<PriceSet>();
        var stock = Create<string>();
        var stockPrice = new StockPrice { Stock = stock, Price = Create<decimal>() };
        var date = prices.Prices.Keys.First();
        var newPrices = new PriceSet
        {
            Prices = new Dictionary<DateTime, HashSet<StockPrice>>
            {
                [date] = new HashSet<StockPrice> { stockPrice }
            }
        };

        // Act
        var result = prices.AddPrices(newPrices, stock);

        // Assert
        result.Prices[date].Should().Contain(stockPrice);
    }

    [Fact]
    public void AddPrices_DoesNotWipeExistingData_IfDateKeyAlreadyExists()
    {
        // Arrange
        var prices = Create<PriceSet>();
        var stock = Create<string>();
        var stockPrice = new StockPrice { Stock = stock, Price = Create<decimal>() };
        var date = prices.Prices.Keys.First();
        var newPrices = new PriceSet
        {
            Prices = new Dictionary<DateTime, HashSet<StockPrice>>
            {
                [date] = new HashSet<StockPrice> { stockPrice }
            }
        };

        // Act
        var result = prices.AddPrices(newPrices, stock);

        // Assert
        result.Prices[date].Should().HaveCount(4);
    }

    [Fact]
    public void AddPrices_DoesNotAddPrice_ForOtherStocks()
    {
        // Arrange
        var prices = Create<PriceSet>();
        var stockPrice = Create<StockPrice>();
        var date = prices.Prices.Keys.First();
        var newPrices = new PriceSet
        {
            Prices = new Dictionary<DateTime, HashSet<StockPrice>>
            {
                [date] = new HashSet<StockPrice> { stockPrice }
            }
        };

        // Act
        var result = prices.AddPrices(newPrices, Create<string>());

        // Assert
        result.Prices[date].Should().HaveCount(3);
    }

    [Fact]
    public void Interpolate_InterpolatesSingleMissingDataPoint()
    {
        // Arrange
        var stock = Create<string>();        
        var stockPrice1 = new StockPrice { Stock = stock, Price = 1m };
        var stockPrice2 = new StockPrice { Stock = stock, Price = 2m };
        var date = Create<DateTime>();
        var prices = new PriceSet
        {
            Prices = new Dictionary<DateTime, HashSet<StockPrice>>
            {
                [date.AddDays(-1)] = new HashSet<StockPrice> { stockPrice1 },
                [date.AddDays(0)] = new HashSet<StockPrice>(),
                [date.AddDays(1)] = new HashSet<StockPrice> { stockPrice2 },
            }
        };

        // Act
        var result = prices.Interpolate(new [] { stock });

        // Assert
        result.Prices.Keys.Should().Contain(date);
        result.Prices[date].Count.Should().Be(1);
        result.Prices[date].Single().Should().BeEquivalentTo(new StockPrice { Stock = stock, Price = 1.5m });
    }

    [Fact]
    public void Interpolate_InterpolatesTwoMissingDataPoints()
    {
        // Arrange
        var stock = Create<string>();        
        var stockPrice0 = new StockPrice { Stock = stock, Price = 0.1m };
        var stockPrice3 = new StockPrice { Stock = stock, Price = 0.4m };
        var date = Create<DateTime>();
        var prices = new PriceSet
        {
            Prices = new Dictionary<DateTime, HashSet<StockPrice>>
            {
                [date.AddDays(0)] = new HashSet<StockPrice> { stockPrice0 },
                [date.AddDays(1)] = new HashSet<StockPrice>(),
                [date.AddDays(2)] = new HashSet<StockPrice>(),
                [date.AddDays(3)] = new HashSet<StockPrice> { stockPrice3 },
            }
        };

        // Act
        var result = prices.Interpolate(new [] { stock });

        // Assert
        result.Prices[date.AddDays(1)].Count.Should().Be(1);
        result.Prices[date.AddDays(1)].Single().Should().BeEquivalentTo(new StockPrice { Stock = stock, Price = 0.2m });
        result.Prices[date.AddDays(2)].Count.Should().Be(1);
        result.Prices[date.AddDays(2)].Single().Should().BeEquivalentTo(new StockPrice { Stock = stock, Price = 0.3m });
    }

    [Fact]
    public void Interpolate_InterpolatesFourMissingDataPoints()
    {
        // Arrange
        var stock = Create<string>();        
        var stockPrice0 = new StockPrice { Stock = stock, Price = 1m };
        var stockPrice5 = new StockPrice { Stock = stock, Price = 2m };
        var date = Create<DateTime>();
        var prices = new PriceSet
        {
            Prices = new Dictionary<DateTime, HashSet<StockPrice>>
            {
                [date.AddDays(0)] = new HashSet<StockPrice> { stockPrice0 },
                [date.AddDays(1)] = new HashSet<StockPrice>(),
                [date.AddDays(2)] = new HashSet<StockPrice>(),
                [date.AddDays(3)] = new HashSet<StockPrice>(),
                [date.AddDays(4)] = new HashSet<StockPrice>(),
                [date.AddDays(5)] = new HashSet<StockPrice> { stockPrice5 },
            }
        };

        // Act
        var result = prices.Interpolate(new [] { stock });

        // Assert
        result.Prices[date.AddDays(1)].Count.Should().Be(1);
        result.Prices[date.AddDays(1)].Single().Should().BeEquivalentTo(new StockPrice { Stock = stock, Price = 1.2m });
        result.Prices[date.AddDays(2)].Count.Should().Be(1);
        result.Prices[date.AddDays(2)].Single().Should().BeEquivalentTo(new StockPrice { Stock = stock, Price = 1.4m });
        result.Prices[date.AddDays(3)].Count.Should().Be(1);
        result.Prices[date.AddDays(3)].Single().Should().BeEquivalentTo(new StockPrice { Stock = stock, Price = 1.6m });
        result.Prices[date.AddDays(4)].Count.Should().Be(1);
        result.Prices[date.AddDays(4)].Single().Should().BeEquivalentTo(new StockPrice { Stock = stock, Price = 1.8m });
    }

    [Fact]
    public void Interpolate_DoesNotExtrapolate()
    {
        // Arrange
        var stock = Create<string>();        
        var stockPrice2 = new StockPrice { Stock = stock, Price = 2m };
        var stockPrice4 = new StockPrice { Stock = stock, Price = 4m };
        var date = Create<DateTime>();
        var prices = new PriceSet
        {
            Prices = new Dictionary<DateTime, HashSet<StockPrice>>
            {
                [date.AddDays(0)] = new HashSet<StockPrice>(),
                [date.AddDays(1)] = new HashSet<StockPrice>(),
                [date.AddDays(2)] = new HashSet<StockPrice> { stockPrice2 },
                [date.AddDays(3)] = new HashSet<StockPrice>(),
                [date.AddDays(4)] = new HashSet<StockPrice> { stockPrice4 },
                [date.AddDays(5)] = new HashSet<StockPrice>(),
                [date.AddDays(6)] = new HashSet<StockPrice>()
            }
        };

        // Act
        var result = prices.Interpolate(new [] { stock });

        // Assert
        result.Prices[date.AddDays(0)].Count.Should().Be(0);
        result.Prices[date.AddDays(1)].Count.Should().Be(0);
        result.Prices[date.AddDays(2)].Count.Should().Be(1);
        result.Prices[date.AddDays(3)].Count.Should().Be(1);
        result.Prices[date.AddDays(4)].Count.Should().Be(1);
        result.Prices[date.AddDays(5)].Count.Should().Be(0);
        result.Prices[date.AddDays(6)].Count.Should().Be(0);
    }

    [Fact]
    public void Interpolate_InterpolatesAllStocks()
    {
        // Arrange
        var stock1 = Create<string>();        
        var stock2 = Create<string>();        
        var stockPrice11 = new StockPrice { Stock = stock1, Price = 1m };
        var stockPrice12 = new StockPrice { Stock = stock1, Price = 2m };
        var stockPrice21 = new StockPrice { Stock = stock2, Price = 2m };
        var stockPrice22 = new StockPrice { Stock = stock2, Price = 3m };
        var date = Create<DateTime>();
        var prices = new PriceSet
        {
            Prices = new Dictionary<DateTime, HashSet<StockPrice>>
            {
                [date.AddDays(0)] = new HashSet<StockPrice> { stockPrice11 },
                [date.AddDays(1)] = new HashSet<StockPrice>(),
                [date.AddDays(2)] = new HashSet<StockPrice> { stockPrice12 },
                [date.AddDays(3)] = new HashSet<StockPrice>(),
                [date.AddDays(4)] = new HashSet<StockPrice> { stockPrice21 },
                [date.AddDays(5)] = new HashSet<StockPrice>(),
                [date.AddDays(6)] = new HashSet<StockPrice> { stockPrice22 },
            }
        };

        // Act
        var result = prices.Interpolate(new [] { stock1, stock2 });

        // Assert
        result.Prices[date.AddDays(0)].Count.Should().Be(1);
        result.Prices[date.AddDays(1)].Count.Should().Be(1);
        result.Prices[date.AddDays(1)].Single().Should().BeEquivalentTo(new StockPrice { Stock = stock1, Price = 1.5m });
        result.Prices[date.AddDays(2)].Count.Should().Be(1);
        result.Prices[date.AddDays(3)].Count.Should().Be(0);
        result.Prices[date.AddDays(4)].Count.Should().Be(1);
        result.Prices[date.AddDays(5)].Count.Should().Be(1);
        result.Prices[date.AddDays(5)].Single().Should().BeEquivalentTo(new StockPrice { Stock = stock2, Price = 2.5m });
        result.Prices[date.AddDays(6)].Count.Should().Be(1);
    }

    [Fact]
    public void ConvertToCsv_CreatesHeaderRowWithSuppliedHeaderValues()
    {
        // Arrange
        var priceSet = Create<PriceSet>();
        var headers = Create<string[]>();

        // Act
        var result = priceSet.ConvertToCsv(headers);

        // Assert
        var lines = result.Split('\n');
        lines.Count().Should().BeGreaterThan(0);
        lines.First().Should().BeEquivalentTo(string.Join(",", headers));
    }

    [Fact]
    public void ConvertToCsv_CreatesDateColumn()
    {
        // Arrange
        var prices = Create<PriceSet>();
        var headers = Create<string[]>();

        // Act
        var result = prices.ConvertToCsv(headers);

        // Assert
        var split = result.Split('\n');
        var lines = split.Take(split.Length - 1); // because the last line finishes with a newline char
        lines.Count().Should().Be(prices.Prices.Keys.Count() + 1);
        foreach(var line in lines.Skip(1))
            prices.Prices.Keys.Contains(DateTime.Parse(line.Split(",").First()));
    }   

    [Fact]
    public void ConvertToCsv_PopulatesMatchingPriceSetData()
    {
        // Arrange
        var stock1 = Create<string>();
        var stock2 = Create<string>();
        var stock3 = Create<string>();
        var headers = new [] { "Date", stock1, "", stock2, "CHEESE", stock3 };
        var stockPrice1 = new StockPrice { Stock = stock1, Price = 1.234m };
        var stockPrice2 = new StockPrice { Stock = stock2, Price = 2.345m };
        var stockPrice3 = new StockPrice { Stock = stock3, Price = 3.456m };
        var stockPrice4 = new StockPrice { Stock = stock3, Price = 4.567m };
        var date = Create<DateTime>();
        var prices = new PriceSet
        {
            Prices = new Dictionary<DateTime, HashSet<StockPrice>>
            {
                [date.AddDays(0)] = new HashSet<StockPrice> { stockPrice1, stockPrice3 },
                [date.AddDays(1)] = new HashSet<StockPrice>(),
                [date.AddDays(2)] = new HashSet<StockPrice> { stockPrice2 },
                [date.AddDays(3)] = new HashSet<StockPrice> { stockPrice4 },
            }
        };        

        // Act
        var result = prices.ConvertToCsv(headers);

        // Assert
        var split = result.Split('\n');
        var line = split[1];
        var expectedData = new [] { date.ToString("yyyy-MM-dd"), $"{stockPrice1.Price}", "", "", "", $"{stockPrice3.Price}" };
        line.Should().BeEquivalentTo(string.Join(",", expectedData));
        line = split[2];
        expectedData = new [] { date.AddDays(1).ToString("yyyy-MM-dd"), "", "", "", "", "" };
        line.Should().BeEquivalentTo(string.Join(",", expectedData));
        line = split[3];
        expectedData = new [] { date.AddDays(2).ToString("yyyy-MM-dd"), "", "", $"{stockPrice2.Price}", "", "" };
        line.Should().BeEquivalentTo(string.Join(",", expectedData));
        line = split[4];
        expectedData = new [] { date.AddDays(3).ToString("yyyy-MM-dd"), "", "", "", "", $"{stockPrice4.Price}" };
        line.Should().BeEquivalentTo(string.Join(",", expectedData));
    }   
}