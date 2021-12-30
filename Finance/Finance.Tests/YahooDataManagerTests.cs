using Finance.Data;
using Finance.Utils;
using Finance.Domain.Yahoo;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests;

public class YahooDataManagerTests : TestFixture<YahooDataManager>
{
    private const string GE = "GE";
    private const string DDD = "DDD";
    private const string SGE = "SGE";

    private readonly Mock<IWebDataClient> _mockWebDataClient = new Mock<IWebDataClient>();
    private readonly Mock<IFileIO> _mockFileIO = new Mock<IFileIO>();

    public YahooDataManagerTests()
    {
        var response = Create<Response>();
        foreach(var price in response.prices)
            price.date *= 1000000;

        _mockWebDataClient
            .Setup(client => client.GetYahooApiData(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<bool>()))
            .ReturnsAsync(response);

        Inject(_mockWebDataClient.Object);
        Inject(_mockFileIO.Object);
    }

    [Fact]
    public async Task GeneratePriceDataFromApi_InvokesYahooApiWithCorrectArgs()
    {
        // Arrange
        var dates = Create<DateTime[]>().OrderBy(d => d).ToArray();
        var stocks = Create<string[]>();
        var writeFlag = Create<bool>();

        // Act
        await Sut.GeneratePriceDataFromApi(dates, stocks, writeFlag);

        // Assert
        var startDate = dates.First().ToUnixTimeStamp();
        var endDate = dates.Last().ToUnixTimeStamp();
        foreach (var stock in stocks)
            _mockWebDataClient.Verify(client => client.GetYahooApiData(stock, startDate, endDate, writeFlag), Times.Once);

        _mockWebDataClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GeneratePriceDataFromApi_GeneratesCsvWithHeaders()
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
        await Sut.GeneratePriceDataFromApi(dates, stocks);

        // Assert
        writePayload.Should().NotBeNull();
        var headerRow = writePayload.Split('\n').First();
        headerRow.Should().Be(string.Join(",", Constants.Headers));
    }

    [Fact]
    public async Task GeneratePriceDataFromApi_AddsPricesFromEachResponse()
    {
        // Arrange
        var date = Create<DateTime>();
        var unixDate = date.ToUnixTimeStamp();
        var dates = new [] { date };
        var stocks = new [] { GE, DDD };
        var gePrice = new Price { date = (long)unixDate, close = Create<decimal>() };
        var dddPrice = new Price { date = (long)unixDate, close = Create<decimal>() };

        _mockWebDataClient
            .Setup(client => client.GetYahooApiData(GE, unixDate, unixDate, It.IsAny<bool>()))
            .ReturnsAsync(new Response { prices = new [] { gePrice }});

        _mockWebDataClient
            .Setup(client => client.GetYahooApiData(DDD, unixDate, unixDate, It.IsAny<bool>()))
            .ReturnsAsync(new Response { prices = new [] { dddPrice }});

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceDataFromApi(dates, stocks);

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
    public async Task GeneratePriceDataFromApi_InterpolatesData()
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

        _mockWebDataClient
            .Setup(client => client.GetYahooApiData(GE, It.IsAny<double>(), It.IsAny<double>(), It.IsAny<bool>()))
            .ReturnsAsync(new Response { prices = prices });

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var geIndex = Array.FindIndex(headerRow.Split(","), val => val == GE);        
        var data = rows[2].Split(",");
        data[geIndex].Should().Be($"{2.4m}");
    }

    [Fact]
    public async Task GeneratePriceDataFromApi_HandlesExchangeSuffix()
    {
        // Arrange
        var date = Create<DateTime>().Date;
        var unixDate = date.ToUnixTimeStamp();
        var dates = new [] { date };
        var stocks = new [] { SGE + ".L" };
        var price = new Price { date = (long)unixDate, close = Create<decimal>() };

        _mockWebDataClient
            .Setup(client => client.GetYahooApiData(stocks.First(), unixDate, unixDate, It.IsAny<bool>()))
            .ReturnsAsync(new Response { prices = new [] { price }});

        string writePayload = null!;
        _mockFileIO
            .Setup(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string text, string file) => writePayload = text);

        // Act
        await Sut.GeneratePriceDataFromApi(dates, stocks);

        // Assert
        var rows = writePayload.Split('\n');
        var headerRow = rows.First();
        var sgeIndex = Array.FindIndex(headerRow.Split(","), val => val == SGE);

        var data = rows[1].Split(",");        
        data[sgeIndex].Should().Be($"{price.close}");
    }
}