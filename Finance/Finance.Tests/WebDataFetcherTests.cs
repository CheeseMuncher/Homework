using Finance.Data;
using Finance.Utils;
using Finance.Domain.Yahoo;
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

public class WebDataFetcherTests : TestFixture<WebDataFetcher>
{
    private Mock<IFileIO> _mockFileIO = new Mock<IFileIO>();
    private Mock<IHttpRequestFactory> _mockFactory = new Mock<IHttpRequestFactory>();
    private Mock<HttpMessageHandler> _mockHandler = new Mock<HttpMessageHandler>();

    public WebDataFetcherTests()
    {
        _mockFactory
            .Setup(factory => factory.GetYahooFinanceRequest(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .Returns(Create<HttpRequestMessage>());

        _mockHandler.Protected()            
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { });

        Inject(_mockFileIO.Object);
        Inject(_mockFactory.Object);
        Inject(new HttpClient(_mockHandler.Object));
    }

    [Fact]
    public async void GetYahooApiData_FetchesHttpRequestFromFactory()
    {
        // Arrange
        var stock = Create<string>();
        var start = Create<double>();
        var end = Create<double>();

        string stockPayload = null;
        double startPayload = 0;
        double endPayload = 0;
        _mockFactory
            .Setup(factory => factory.GetYahooFinanceRequest(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
            .Callback((string s, double d1, double d2) => 
            {
                stockPayload = s;
                startPayload = d1;
                endPayload = d2;
            })
            .Returns(Create<HttpRequestMessage>());        

        // Act
        await Sut.GetYahooApiData(stock, start, end);        

        // Assert
        stockPayload.Should().Be(stock);
        startPayload.Should().Be(start);
        endPayload.Should().Be(end);
    }

    [Fact]
    public async void GetYahooApiData_SendsFactoryRequestToApi()
    {
            // Arrange
            var factoryRequest = Create<HttpRequestMessage>();
            _mockFactory
                .Setup(factory => factory.GetYahooFinanceRequest(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<double>()))
                .Returns(factoryRequest);        

            HttpRequestMessage payload = null;
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage hrm, CancellationToken ct) => payload = hrm)
                .ReturnsAsync(new HttpResponseMessage { });

            // Act
            await Sut.GetYahooApiData(Create<string>(), Create<double>(), Create<double>());

            // Assert
            payload.Should().NotBeNull();
            payload.Should().BeEquivalentTo(factoryRequest);
    }

    [Fact]
    public async void GetYahooApiData_DoesNotWriteResponseDataToFile_IfWriteFlagNotSet()
    {
            // Arrange

            // Act
            await Sut.GetYahooApiData(Create<string>(), Create<double>(), Create<double>());

            // Assert
            _mockFileIO.Verify(io => io.WriteText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public async void GetYahooApiData_WritesResponseDataToFile_IfFlagSet()
    {
            // Arrange
            var stock = Create<string>();
            var content = Create<string>();
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())                
                .ReturnsAsync(new HttpResponseMessage 
                { 
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content)
                });

            // Act
            await Sut.GetYahooApiData(stock, Create<double>(), Create<double>(), true);

            // Assert
            _mockFileIO.Verify(io => io.WriteText(content, It.Is<string>(str => str.Contains(stock))), Times.Once);
            _mockFileIO.VerifyNoOtherCalls();
    }

    [Fact]
    public async void GetYahooApiData_DeserialisesResponseData()
    {
            // Arrange
            var response = Create<Response>();
            var content = JsonSerializer.Serialize(response);
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())                
                .ReturnsAsync(new HttpResponseMessage 
                { 
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content)
                });

            // Act
            var result = await Sut.GetYahooApiData(Create<string>(), Create<double>(), Create<double>(), true);

            // Assert
            result.Should().BeEquivalentTo(response);
    }
}