using Finance.Data;
using Finance.Utils;
using Finance.Domain.Yahoo.Models;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Finance.Tests;

public class WebDataClientTests : TestFixture<WebDataClient>
{
    private readonly Mock<IFileIO> _mockFileIO = new Mock<IFileIO>();
    private readonly Mock<IHttpRequestFactory> _mockFactory = new Mock<IHttpRequestFactory>();
    private readonly Mock<HttpMessageHandler> _mockHandler = new Mock<HttpMessageHandler>();

    public WebDataClientTests()
    {
        _mockFactory
            .Setup(factory => factory.GetHistoryDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Returns(Create<HttpRequestMessage>());

        _mockFactory
            .Setup(factory => factory.GetChartDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Returns(Create<HttpRequestMessage>());

        _mockHandler.Protected()            
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { Content = new StringContent("{}")});

        Inject(_mockFileIO.Object);
        Inject(_mockFactory.Object);
        Inject(new HttpClient(_mockHandler.Object));
    }

    [Fact]
    public async void GetYahooHistoryData_FetchesHttpRequestFromFactory()
    {
        // Arrange
        var stock = Create<string>();
        var start = Create<long>();
        var end = Create<long>();

        string stockPayload = null!;
        long startPayload = 0;
        long endPayload = 0;
        _mockFactory
            .Setup(factory => factory.GetHistoryDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Callback((string s, long l1, long l2) => 
            {
                stockPayload = s;
                startPayload = l1;
                endPayload = l2;
            })
            .Returns(Create<HttpRequestMessage>());

        // Act
        await Sut.GetYahooHistoryData(stock, start, end);

        // Assert
        stockPayload.Should().Be(stock);
        startPayload.Should().Be(start);
        endPayload.Should().Be(end);
    }

    [Fact]
    public async void GetYahooHistoryData_SendsFactoryRequestToApi()
    {
        // Arrange
        var factoryRequest = Create<HttpRequestMessage>();
        _mockFactory
            .Setup(factory => factory.GetHistoryDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Returns(factoryRequest);        

        HttpRequestMessage payload = null!;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback((HttpRequestMessage hrm, CancellationToken ct) => payload = hrm)
            .ReturnsAsync(new HttpResponseMessage { Content = new StringContent("{}")});

        // Act
        await Sut.GetYahooHistoryData(Create<string>(), Create<long>(), Create<long>());

        // Assert
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(factoryRequest);
    }

    [Fact]
    public async void GetYahooHistoryData_DoesNotWriteResponseDataToFile_IfWriteFlagNotSet()
    {
        // Arrange

        // Act
        await Sut.GetYahooHistoryData(Create<string>(), Create<long>(), Create<long>());

        // Assert
        _mockFileIO.Verify(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public async void GetYahooHistoryData_WritesResponseDataToFile_IfFlagSet()
    {
        // Arrange
        var stock = Create<string>();
        var response = Create<HistoryResponse>();
        var content = JsonSerializer.Serialize(response);
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())                
            .ReturnsAsync(new HttpResponseMessage 
            { 
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content)
            });

        // Act
        await Sut.GetYahooHistoryData(stock, Create<long>(), Create<long>(), true);

        // Assert
        _mockFileIO.Verify(io => io.WriteText(content, It.Is<string>(str => str.Contains(stock))), Times.Once);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public async void GetYahooHistoryData_DeserialisesResponseData()
    {
        // Arrange
        var response = Create<HistoryResponse>();
        var content = JsonSerializer.Serialize(response);
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())                
            .ReturnsAsync(new HttpResponseMessage 
            { 
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content)
            });

        // Act
        var result = await Sut.GetYahooHistoryData(Create<string>(), Create<long>(), Create<long>(), true);

        // Assert
        result.Should().BeEquivalentTo(response);
    }





    [Fact]
    public async void GetYahooChartData_FetchesHttpRequestFromFactory()
    {
        // Arrange
        var stock = Create<string>();
        var start = Create<long>();
        var end = Create<long>();

        string stockPayload = null!;
        long startPayload = 0;
        long endPayload = 0;
        _mockFactory
            .Setup(factory => factory.GetChartDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Callback((string s, long l1, long l2) => 
            {
                stockPayload = s;
                startPayload = l1;
                endPayload = l2;
            })
            .Returns(Create<HttpRequestMessage>());

        // Act
        await Sut.GetYahooChartData(stock, start, end);

        // Assert
        stockPayload.Should().Be(stock);
        startPayload.Should().Be(start);
        endPayload.Should().Be(end);
    }

    [Fact]
    public async void GetYahooChartData_SendsFactoryRequestToApi()
    {
        // Arrange
        var factoryRequest = Create<HttpRequestMessage>();
        _mockFactory
            .Setup(factory => factory.GetChartDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Returns(factoryRequest);        

        HttpRequestMessage payload = null!;
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback((HttpRequestMessage hrm, CancellationToken ct) => payload = hrm)
            .ReturnsAsync(new HttpResponseMessage { Content = new StringContent("{}")});

        // Act
        await Sut.GetYahooChartData(Create<string>(), Create<long>(), Create<long>());

        // Assert
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(factoryRequest);
    }

    [Fact]
    public async void GetYahooChartData_DoesNotWriteResponseDataToFile_IfWriteFlagNotSet()
    {
        // Arrange

        // Act
        await Sut.GetYahooChartData(Create<string>(), Create<long>(), Create<long>());

        // Assert
        _mockFileIO.Verify(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public async void GetYahooChartData_WritesResponseDataToFile_IfFlagSet()
    {
        // Arrange
        var stock = Create<string>();
        var response = Create<ChartResponse>();
        var content = JsonSerializer.Serialize(response);
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())                
            .ReturnsAsync(new HttpResponseMessage 
            { 
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content)
            });

        // Act
        await Sut.GetYahooChartData(stock, Create<long>(), Create<long>(), true);

        // Assert
        _mockFileIO.Verify(io => io.WriteText(content, It.Is<string>(str => str.Contains(stock))), Times.Once);
        _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public async void GetYahooChartData_DeserialisesResponseData()
    {
        // Arrange
        var response = Create<ChartResponse>();
        var content = JsonSerializer.Serialize(response);
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())                
            .ReturnsAsync(new HttpResponseMessage 
            { 
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(content)
            });

        // Act
        var result = await Sut.GetYahooChartData(Create<string>(), Create<long>(), Create<long>(), true);

        // Assert
        result.Should().BeEquivalentTo(response);
    }
}