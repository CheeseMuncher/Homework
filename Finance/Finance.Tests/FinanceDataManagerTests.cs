using Finance.Data;
using Finance.Utils;
using Finance.Domain.Yahoo;
using Finance.Domain.Yahoo.Models;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests;

public class FinanceDataManagerTests : TestFixture<FinanceDataManager>
{
    private const string GE = "GE";
    private const string DDD = "DDD";
    private const string SGE = "SGE";

    private readonly Mock<IWebDataClient> _mockWebClient = new Mock<IWebDataClient>();
    private readonly Mock<IFileDataClient> _mockFileClient = new Mock<IFileDataClient>();
    private readonly Mock<IFileIO> _mockFileIO = new Mock<IFileIO>();

    public FinanceDataManagerTests()
    {
        var response = Create<HistoryResponse>();
        foreach(var price in response.prices)
            price.date = ValidUnixDate(Create<long>());

        _mockWebClient
            .Setup(client => client.GetYahooHistoryData(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<bool>()))
            .ReturnsAsync(response);

        _mockWebClient
            .Setup(client => client.GetYahooChartData(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<bool>()))
            .ReturnsAsync(CreateChartResponseWithSinglePrice(Create<string>(), Create<long>(), Create<decimal>()));

        _mockFileClient
            .Setup(client => client.GetYahooFileHistoryData(It.IsAny<string>()))
            .Returns(response);

        _mockFileClient
            .Setup(client => client.GetYahooFileChartData(It.IsAny<string>()))
            .Returns(CreateChartResponseWithSinglePrice(Create<string>(), Create<long>(), Create<decimal>()));

        Inject(_mockWebClient.Object);
        Inject(_mockFileClient.Object);
        Inject(_mockFileIO.Object);
    }

    [Fact]
    public async Task GeneratePriceHistoryDataFromApi_InvokesWebClientWithCorrectArgs()
    {
        // Arrange
        var dates = Create<DateTime[]>().OrderBy(d => d).ToArray();
        var stocks = Create<string[]>();
        var writeFlag = Create<bool>();

        // Act
        await Sut.GeneratePriceHistoryDataFromApi(dates, stocks, writeFlag);

        // Assert
        var startDate = (long)dates.First().ToUnixTimeStamp();
        var endDate = (long)dates.Last().ToUnixTimeStamp();
        foreach (var stock in stocks)
            _mockWebClient.Verify(client => client.GetYahooHistoryData(stock, startDate, endDate, writeFlag), Times.Once);

        _mockWebClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GeneratePriceHistoryDataFromApi_GeneratesCsvWithHeaders()
    {
        // Arrange
        var date = Create<DateTime>();        
        var dates = new [] { date };
        var stocks = Create<string[]>();
        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceHistoryDataFromApi(dates, stocks);

        // Assert
        writePayload.Should().NotBeNull();
        var headerRow = writePayload.Split('\n').First();
        headerRow.Should().Be(string.Join(",", QuoteKeys.Headers));
    }

    [Fact]
    public async Task GeneratePriceHistoryDataFromApi_AddsPricesFromEachResponse()
    {
        // Arrange
        var date = Create<DateTime>();
        var unixDate = (long)date.ToUnixTimeStamp();
        var dates = new [] { date };
        var stocks = new [] { GE, DDD };
        var gePrice = new Price { date = (long)unixDate, close = Create<decimal>() };
        var dddPrice = new Price { date = (long)unixDate, close = Create<decimal>() };

        _mockWebClient
            .Setup(client => client.GetYahooHistoryData(GE, unixDate, unixDate, It.IsAny<bool>()))
            .ReturnsAsync(new HistoryResponse { prices = new [] { gePrice }});

        _mockWebClient
            .Setup(client => client.GetYahooHistoryData(DDD, unixDate, unixDate, It.IsAny<bool>()))
            .ReturnsAsync(new HistoryResponse { prices = new [] { dddPrice }});

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceHistoryDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var geIndex = Array.FindIndex(headerRow.Split(","), val => val == GE);
        var dddIndex = Array.FindIndex(headerRow.Split(","), val => val == DDD);

        var dataRow = rows[1];
        var data = dataRow.Split(",");
        data[geIndex].Should().Be($"{gePrice.close}");
        data[dddIndex].Should().Be($"{dddPrice.close}");
    }

    [Fact]
    public async Task GeneratePriceHistoryDataFromApi_InterpolatesData()
    {
        // Arrange
        var date = Create<DateTime>().Date;        
        var dates = new [] { date, date.AddDays(1), date.AddDays(2) };
        var stocks = new [] { GE };
        var prices = new []
        {
            new Price { date = (long)date.AddDays(0).ToUnixTimeStamp(), close = 1.2m },
            new Price { date = (long)date.AddDays(1).ToUnixTimeStamp(), close = 0m },
            new Price { date = (long)date.AddDays(2).ToUnixTimeStamp(), close = 3.6m },
        };

        _mockWebClient
            .Setup(client => client.GetYahooHistoryData(GE, It.IsAny<long>(), It.IsAny<long>(), It.IsAny<bool>()))
            .ReturnsAsync(new HistoryResponse { prices = prices });

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceHistoryDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var index = Array.FindIndex(headerRow.Split(","), val => val == GE);        
        var data = rows[2].Split(",");
        data[index].Should().Be($"{2.4m}");
    }
    
    [Fact]
    public async Task GeneratePriceHistoryDataFromApi_InterpolatesDataWithSuffixHandling()
    {
        // Arrange
        var date = Create<DateTime>().Date;        
        var dates = new [] { date, date.AddDays(1), date.AddDays(2) };
        var stocks = new [] { SGE + ".L" };
        var prices = new []
        {
            new Price { date = (long)date.AddDays(0).ToUnixTimeStamp(), close = 1.2m },
            new Price { date = (long)date.AddDays(1).ToUnixTimeStamp(), close = 0m },
            new Price { date = (long)date.AddDays(2).ToUnixTimeStamp(), close = 3.6m },
        };

        _mockWebClient
            .Setup(client => client.GetYahooHistoryData(stocks.First(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<bool>()))
            .ReturnsAsync(new HistoryResponse { prices = prices });

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceHistoryDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var index = Array.FindIndex(headerRow.Split(","), val => val == SGE);        
        var data = rows[2].Split(",");
        data[index].Should().Be($"{2.4m}");
    }
    
    [Fact]
    public async Task GeneratePriceHistoryDataFromApi_HandlesExchangeSuffix()
    {
        // Arrange
        var date = Create<DateTime>().Date;
        var unixDate = (long)date.ToUnixTimeStamp();
        var dates = new [] { date };
        var stocks = new [] { SGE + ".L" };
        var price = new Price { date = unixDate, close = Create<decimal>() };

        _mockWebClient
            .Setup(client => client.GetYahooHistoryData(stocks.First(), unixDate, unixDate, It.IsAny<bool>()))
            .ReturnsAsync(new HistoryResponse { prices = new [] { price }});

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceHistoryDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var sgeIndex = Array.FindIndex(headerRow.Split(","), val => val == SGE);

        var data = rows[1].Split(",");        
        data[sgeIndex].Should().Be($"{price.close}");
    }

    [Fact]
    public async Task GeneratePriceChartDataFromApi_InvokesWebClientWithCorrectArgs()
    {
        // Arrange
        var dates = Create<DateTime[]>().OrderBy(d => d).ToArray();
        var stocks = Create<string[]>();
        var writeFlag = Create<bool>();

        // Act
        await Sut.GeneratePriceChartDataFromApi(dates, stocks, writeFlag);

        // Assert
        var startDate = (long)dates.First().ToUnixTimeStamp();
        var endDate = (long)dates.Last().ToUnixTimeStamp();
        foreach (var stock in stocks)
            _mockWebClient.Verify(client => client.GetYahooChartData(stock, startDate, endDate, writeFlag), Times.Once);

        _mockWebClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GeneratePriceChartDataFromApi_GeneratesCsvWithHeaders()
    {
        // Arrange
        var date = Create<DateTime>();        
        var dates = new [] { date };
        var stocks = Create<string[]>();
        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceChartDataFromApi(dates, stocks);

        // Assert
        writePayload.Should().NotBeNull();
        var headerRow = writePayload.Split('\n').First();
        headerRow.Should().Be(string.Join(",", QuoteKeys.Headers));
    }

    [Fact]
    public async Task GeneratePriceChartDataFromApi_AddsPricesFromEachResponse()
    {
        // Arrange
        var date = Create<DateTime>().Date;
        var unixDate = (long)date.ToUnixTimeStamp();
        var dates = new [] { date };
        var stocks = new [] { GE, DDD };
        var gePrice = Create<decimal>();
        var dddPrice = Create<decimal>();

        _mockWebClient
            .Setup(client => client.GetYahooChartData(GE, unixDate, unixDate, It.IsAny<bool>()))
            .ReturnsAsync(CreateChartResponseWithSinglePrice(GE, unixDate, gePrice));

        _mockWebClient
            .Setup(client => client.GetYahooChartData(DDD, unixDate, unixDate, It.IsAny<bool>()))
            .ReturnsAsync(CreateChartResponseWithSinglePrice(DDD, unixDate, dddPrice));

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceChartDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var geIndex = Array.FindIndex(headerRow.Split(","), val => val == GE);
        var dddIndex = Array.FindIndex(headerRow.Split(","), val => val == DDD);

        var dataRow = rows[1];
        var data = dataRow.Split(",");
        data[geIndex].Should().Be($"{gePrice}");
        data[dddIndex].Should().Be($"{dddPrice}");
    }

    [Fact]
    public async Task GeneratePriceChartDataFromApi_InterpolatesData()
    {
        // Arrange
        var date = Create<DateTime>().Date;        
        var dates = new [] { date, date.AddDays(1), date.AddDays(2) };
        var yahooDates = dates.Select(d => (long)d.ToUnixTimeStamp()).ToArray();
        var stocks = new [] { GE };
        var prices = new [] { 1.2m, 0m, 3.6m };

        _mockWebClient
            .Setup(client => client.GetYahooChartData(GE, It.IsAny<long>(), It.IsAny<long>(), It.IsAny<bool>()))
            .ReturnsAsync(CreateChartResponseWithMultiplePrices(GE, yahooDates, prices));

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceChartDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var index = Array.FindIndex(headerRow.Split(","), val => val == GE);        
        var data = rows[2].Split(",");
        data[index].Should().Be($"{2.4m}");
    }


    [Fact]
    public async Task GeneratePriceChartDataFromApi_InterpolatesDataWithSuffixHandling()
    {
        // Arrange
        var date = Create<DateTime>().Date;        
        var dates = new [] { date, date.AddDays(1), date.AddDays(2) };
        var yahooDates = dates.Select(d => (long)d.ToUnixTimeStamp()).ToArray();
        var stocks = new [] { SGE + ".L" };
        var prices = new [] { 1.2m, 0m, 3.6m };

        _mockWebClient
            .Setup(client => client.GetYahooChartData(stocks.First(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<bool>()))
            .ReturnsAsync(CreateChartResponseWithMultiplePrices(SGE, yahooDates, prices));

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceChartDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var index = Array.FindIndex(headerRow.Split(","), val => val == SGE);        
        var data = rows[2].Split(",");
        data[index].Should().Be($"{2.4m}");
    }

    [Fact]
    public async Task GeneratePriceChartDataFromApi_HandlesExchangeSuffix()
    {
        // Arrange
        var date = Create<DateTime>().Date;
        var unixDate = (long)date.ToUnixTimeStamp();
        var dates = new [] { date };
        var stocks = new [] { SGE + ".L" };
        var price = Create<decimal>();

        _mockWebClient
            .Setup(client => client.GetYahooChartData(stocks.First(), unixDate, unixDate, It.IsAny<bool>()))
            .ReturnsAsync(CreateChartResponseWithSinglePrice(stocks.First(), unixDate, price));

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceChartDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var sgeIndex = Array.FindIndex(headerRow.Split(","), val => val == SGE);

        var data = rows[1].Split(",");        
        data[sgeIndex].Should().Be($"{price}");
    }

    [Fact]
    public void GeneratePriceHistoryDataFromFile_InvokesFileClientWithCorrectArgs()
    {
        // Arrange
        var file = Create<string>();

        // Act
        Sut.GeneratePriceHistoryDataFromFile(file, Create<string>());

        // Assert
        _mockFileClient.Verify(client => client.GetYahooFileHistoryData(file), Times.Once);
        _mockFileClient.VerifyNoOtherCalls();
    }

    [Fact]
    public void GeneratePriceHistoryDataFromFile_GeneratesCsvWithHeaders()
    {
        // Arrange
        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        Sut.GeneratePriceHistoryDataFromFile(Create<string>(), Create<string>());

        // Assert
        writePayload.Should().NotBeNull();
        var headerRow = writePayload.Split('\n').First();
        headerRow.Should().Be(string.Join(",", QuoteKeys.Headers));
    }

    [Fact]
    public void GeneratePriceHistoryDataFromFile_AddsPricesFromFileData()
    {
        // Arrange
        var file = Create<string>();
        var date = (long)Create<DateTime>().Date.ToUnixTimeStamp();
        var price = new Price { date = date, close = Create<decimal>() };

        _mockFileClient
            .Setup(client => client.GetYahooFileHistoryData(It.IsAny<string>()))
            .Returns(new HistoryResponse { prices = new [] { price }});

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        Sut.GeneratePriceHistoryDataFromFile(file, GE);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var index = Array.FindIndex(headerRow.Split(","), val => val == GE);        
        var data = rows[1].Split(",").ToArray();
        data[index].Should().Be($"{price.close}");
    }

    [Fact]
    public void GeneratePriceHistoryDataFromFile_GeneratesOneRowPerDate()
    {
        // Arrange
        var date = Create<DateTime>().Date;
        _mockFileClient
            .Setup(client => client.GetYahooFileHistoryData(It.IsAny<string>()))
            .Returns(new HistoryResponse { prices = new [] 
            {
                new Price { date = (long)date.AddDays(1).ToUnixTimeStamp(), close = Create<decimal>() },
                new Price { date = (long)date.AddDays(2).ToUnixTimeStamp(), close = Create<decimal>() },
                new Price { date = (long)date.AddDays(3).ToUnixTimeStamp(), close = Create<decimal>() }
            }});

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        Sut.GeneratePriceHistoryDataFromFile(Create<string>(), GE);

        // Assert
        var rows = writePayload.Split('\n');
        for (int i = 1; i <= 3; i++)   
            rows[i].Split(",").First().Should().Be(date.AddDays(i).ToString("yyyy-MM-dd"));
    }

    [Fact]
    public void GeneratePriceHistoryDataFromFile_InterpolatesData()
    {
        // Arrange
        var date = Create<DateTime>().Date;        
        var stocks = new [] { GE };

        _mockFileClient
            .Setup(client => client.GetYahooFileHistoryData(It.IsAny<string>()))
            .Returns(new HistoryResponse { prices = new [] 
            {
                new Price { date = (long)date.AddDays(0).ToUnixTimeStamp(), close = 1.2m },
                new Price { date = (long)date.AddDays(1).ToUnixTimeStamp(), close = 0m },
                new Price { date = (long)date.AddDays(2).ToUnixTimeStamp(), close = 3.6m }
            }});

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        Sut.GeneratePriceHistoryDataFromFile(Create<string>(), GE);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var index = Array.FindIndex(headerRow.Split(","), val => val == GE);        
        var data = rows[2].Split(",");
        data[index].Should().Be($"{2.4m}");
    }

    [Fact]
    public void GeneratePriceChartDataFromFile_InvokesFileClientWithCorrectArgs()
    {
        // Arrange
        var file = Create<string>();

        // Act
        Sut.GeneratePriceChartDataFromFile(file, Create<string>());

        // Assert
        _mockFileClient.Verify(client => client.GetYahooFileChartData(file), Times.Once);
        _mockFileClient.VerifyNoOtherCalls();
    }

    [Fact]
    public void GeneratePriceChartDataFromFile_GeneratesCsvWithHeaders()
    {
        // Arrange
        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        Sut.GeneratePriceChartDataFromFile(Create<string>(), Create<string>());

        // Assert
        writePayload.Should().NotBeNull();
        var headerRow = writePayload.Split('\n').First();
        headerRow.Should().Be(string.Join(",", QuoteKeys.Headers));
    }

    [Fact]
    public void GeneratePriceChartDataFromFile_AddsPricesFromFileData()
    {
        // Arrange
        var file = Create<string>();
        var date = (long)Create<DateTime>().Date.ToUnixTimeStamp();
        var price = Create<decimal>();

        _mockFileClient
            .Setup(client => client.GetYahooFileChartData(It.IsAny<string>()))
            .Returns(CreateChartResponseWithSinglePrice(GE, date, price));

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        Sut.GeneratePriceChartDataFromFile(file, GE);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var index = Array.FindIndex(headerRow.Split(","), val => val == GE);        
        var data = rows[1].Split(",").ToArray();
        data[index].Should().Be($"{price}");
    }

    [Fact]
    public void GeneratePriceChartDataFromFile_GeneratesOneRowPerDate()
    {
        // Arrange
        var date = new DateTime(2016, 01, 01).AddDays(Create<int>()).Date;
        var dates = new [] 
        { 
            (long)date.AddDays(1).ToUnixTimeStamp(),
            (long)date.AddDays(2).ToUnixTimeStamp(),
            (long)date.AddDays(3).ToUnixTimeStamp()
        };
        var prices = new [] { Create<decimal>(), Create<decimal>(), Create<decimal>() };
        _mockFileClient
            .Setup(client => client.GetYahooFileChartData(It.IsAny<string>()))
            .Returns(CreateChartResponseWithMultiplePrices(Create<string>(), dates, prices));

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        Sut.GeneratePriceChartDataFromFile(Create<string>(), GE);

        // Assert
        var rows = writePayload.Split('\n');
        for (int i = 1; i <= 3; i++)   
            rows[i].Split(",").First().Should().Be(date.AddDays(i).ToString("yyyy-MM-dd"));
    }

    [Fact]
    public void GeneratePriceChartDataFromFile_InterpolatesData()
    {
        // Arrange
        var date = new DateTime(2016, 01, 01).AddDays(Create<int>()).Date;
        var stocks = new [] { GE };
        var dates = new [] 
        { 
            (long)date.AddDays(1).ToUnixTimeStamp(),
            (long)date.AddDays(2).ToUnixTimeStamp(),
            (long)date.AddDays(3).ToUnixTimeStamp()
        };
        var prices = new [] { 1.2m, 0, 3.6m };
        _mockFileClient
            .Setup(client => client.GetYahooFileChartData(It.IsAny<string>()))
            .Returns(CreateChartResponseWithMultiplePrices(Create<string>(), dates, prices));


        _mockFileClient
            .Setup(client => client.GetYahooFileChartData(It.IsAny<string>()))
            .Returns(CreateChartResponseWithMultiplePrices(GE, dates, prices));

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        Sut.GeneratePriceChartDataFromFile(Create<string>(), GE);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var index = Array.FindIndex(headerRow.Split(","), val => val == GE);        
        var data = rows[2].Split(",");
        data[index].Should().Be($"{2.4m}");
    }

    private ChartResponse CreateChartResponseWithSinglePrice(string stock, long date, decimal price) =>
        new ChartResponse 
            {
                chart = new Chart
                {
                    
                    result = new [] { new Result
                    {
                        meta = new Dictionary<string, object> { ["symbol"] = stock },
                        timestamp = new [] { date },
                        indicators = new Indicators 
                        {
                            quote = new [] { new Quote { close = new [] { price as decimal? } } }
                        }
                    }}
                }
            };

    private ChartResponse CreateChartResponseWithMultiplePrices(string stock, long[] dates, decimal[] prices) =>
        new ChartResponse 
            {
                chart = new Chart
                {
                    
                    result = new [] { new Result
                    {
                        meta = new Dictionary<string, object> { ["symbol"] = stock },
                        timestamp = dates,
                        indicators = new Indicators 
                        {
                            quote = new [] { new Quote { close = prices.Select(p => p as decimal?).ToArray() } }
                        }
                    }}
                }
            };

    private long ValidUnixDate(long days) =>
        (long)(new DateTime(2016, 01, 01).AddDays(days).ToUnixTimeStamp());
}