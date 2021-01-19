using AutoFixture;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Paymentsense.Coding.Challenge.Api.Services;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Paymentsense.Coding.Challenge.Api.Tests.Services
{
    public class RestCountriesApiTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<HttpMessageHandler> _mockMessageHandler = new Mock<HttpMessageHandler>();
        private readonly HttpClient _httpClient;
        private readonly IRestCountriesApi _sut;

        public RestCountriesApiTests()
        {
            _httpClient = new HttpClient(_mockMessageHandler.Object);
            _sut = new RestCountriesApi(_httpClient);
        }

        [Fact]
        public async Task GetFlagAsync_InvokesRestCountriesApi_WithCorrectArgs()
        {
            // Arrange
            var code = _fixture.Create<string>();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") };
            HttpRequestMessage payload = null;
            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage message, CancellationToken token) => payload = message)
                .ReturnsAsync(response);

            // Act
            await _sut.GetFlagAsync(code);

            // Assert
            payload.Should().NotBeNull();
            payload.Method.Should().Be(HttpMethod.Get);
            payload.RequestUri.Should().Be($"https://restcountries.eu/data/{code}.svg");
        }

        [Fact]
        public async Task GetFlagAsync_InvokesRestCountriesApi_WithCorrectCase()
        {
            // Arrange
            var code = _fixture.Create<string>().ToUpper();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("[]") };
            HttpRequestMessage payload = null;
            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Callback((HttpRequestMessage message, CancellationToken token) => payload = message)
                .ReturnsAsync(response);

            // Act
            await _sut.GetFlagAsync(code);

            // Assert
            payload.Should().NotBeNull();
            payload.RequestUri.Should().Be($"https://restcountries.eu/data/{code.ToLower()}.svg");
        }

        [Fact]
        public async Task GetFlagAsync_MapsResponseData_IfGetAsyncCallSuccessful()
        {
            // Arrange
            var data = _fixture.Create<string>();
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(data) };

            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            // Act
            var result = await _sut.GetFlagAsync(_fixture.Create<string>());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(Encoding.ASCII.GetBytes(data));
        }
    }
}