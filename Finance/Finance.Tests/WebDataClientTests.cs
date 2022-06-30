using Finance.Data;
using Finance.Utils;
using Finance.Domain.Yahoo.Models;
using Finance.Domain.TraderMade.Models;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    private readonly Mock<IHttpRequestFactory> _mockRequestFactory = new Mock<IHttpRequestFactory>();
    private readonly Mock<IHttpClientFactory> _mockClientFactory = new Mock<IHttpClientFactory>();
    
    public WebDataClientTests()
    {
        _mockRequestFactory
            .Setup(factory => factory.GetHistoryDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Returns(Create<HttpRequestMessage>());

        _mockRequestFactory
            .Setup(factory => factory.GetChartDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Returns(Create<HttpRequestMessage>());

        _mockRequestFactory
            .Setup(factory => factory.GetHistoryDataTraderMadeRequest(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => BuildValidRequest());

        SetupMockHandler();

        Inject(_mockFileIO.Object);
        Inject(_mockRequestFactory.Object);
        Inject(_mockClientFactory.Object);
    }

    [Fact]
    public async void GetTraderMadeHistoryData_FetchesHttpRequestsFromFactory()
    {
        // Arrange
        var pair = Create<string>();
        var dates = Create<HashSet<DateTime>>();

        var pairPayload = new HashSet<string>()!;
        var datesPayload = new HashSet<string>();
        _mockRequestFactory
            .Setup(factory => factory.GetHistoryDataTraderMadeRequest(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string s1, string s2) => 
            {
                pairPayload.Add(s1);
                datesPayload.Add(s2);
            })
            .Returns(() => BuildValidRequest());

        // Act
        await Sut.GetTraderMadeHistoryData(pair, dates).ToArrayAsync();

        // Assert
        pairPayload.Should().BeEquivalentTo(new HashSet<string> { pair });
        datesPayload.Should().BeEquivalentTo(dates.Select(d => d.ToString("yyyy-MM-dd")).ToHashSet());
    }

    [Fact]
    public async void GetTraderMadeHistoryData_SendsFactoryRequestsToApi()
    {
        // Arrange
        var count = 0;
        var factoryRequests = Enumerable.Range(0, 3).Select(i => BuildValidRequest()).ToArray();
        _mockRequestFactory
            .Setup(factory => factory.GetHistoryDataTraderMadeRequest(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => factoryRequests[count++]);

        var payload = new HashSet<HttpRequestMessage>();
        SetupMockHandler().Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Callback((HttpRequestMessage hrm, CancellationToken ct) => payload.Add(hrm))
            .ReturnsAsync(() => new HttpResponseMessage { Content = new StringContent("{}")});

        // Act
        await Sut.GetTraderMadeHistoryData(Create<string>(), Create<DateTime[]>()).ToArrayAsync();

        // Assert
        payload.Should().NotBeNull();
        payload.Should().BeEquivalentTo(factoryRequests.ToHashSet());
    }

    [Fact]
    public async void GetTraderMadeHistoryData_DeserialisesResponseData()
    {
        // Arrange
        var count = 0;
        var responses = Create<ForexHistoryResponse[]>();
        SetupMockHandler().Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())                
            .ReturnsAsync(() => new HttpResponseMessage 
            { 
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(responses[count++]))
            });

        // Act
        var result = await Sut.GetTraderMadeHistoryData(Create<string>(), Create<DateTime[]>()).ToArrayAsync();

        // Assert
        result.Should().BeEquivalentTo(responses);
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
        _mockRequestFactory
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
        _mockRequestFactory
            .Setup(factory => factory.GetHistoryDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Returns(factoryRequest);        

        HttpRequestMessage payload = null!;
        SetupMockHandler().Protected()
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
        SetupMockHandler().Protected()
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
        SetupMockHandler().Protected()
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
        _mockRequestFactory
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
        _mockRequestFactory
            .Setup(factory => factory.GetChartDataYahooRequest(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>()))
            .Returns(factoryRequest);        

        HttpRequestMessage payload = null!;
        SetupMockHandler().Protected()
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
        SetupMockHandler().Protected()
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
        SetupMockHandler().Protected()
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

    private HttpRequestMessage BuildValidRequest()
    {
        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"http://www.example.com?query={Create<string>()}")
        };
    }

    private Mock<HttpMessageHandler> SetupMockHandler()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()            
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage { Content = new StringContent("{}")});

        var client = new HttpClient(handler.Object);
        _mockClientFactory
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(client);
        
        return handler;
    }
}