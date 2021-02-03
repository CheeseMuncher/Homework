using AutoFixture;
using FluentAssertions;
using Moq;
using Paymentsense.Coding.Challenge.Api.Models;
using Paymentsense.Coding.Challenge.Api.Services;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Paymentsense.Coding.Challenge.Api.Tests.Services
{
    public class CountryServiceTests
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Mock<IRestCountriesApi> _mockRestApi = new Mock<IRestCountriesApi>();
        private ICountryService _sut;

        public CountryServiceTests()
        {
            _sut = new CountryService(_mockRestApi.Object);
        }

        [Fact]
        public void GetAllCountriesAsync_InvokesRestApi()
        {
            // Act
            _sut.GetAllCountriesAsync();

            // Assert
            _mockRestApi.Verify(api => api.GetAllCountriesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllCountriesAsync_MapsNamesFromApiResult()
        {
            // Arrange
            var response = new[]
            {
                _fixture.Create<Country>(),
                _fixture.Create<Country>(),
                _fixture.Create<Country>()
            };

            _mockRestApi
                .Setup(api => api.GetAllCountriesAsync())
                .ReturnsAsync(response);

            // Act
            var result = await _sut.GetAllCountriesAsync();

            // Assert
            result.Count().Should().Be(3);
            result.Should().BeEquivalentTo(response);
        }

        [Fact]
        public void GetFlag_InvokesRestApi_WithCorrectArguments()
        {
            var code = _fixture.Create<string>();

            // Act
            _sut.GetFlagAsync(code);

            // Assert
            _mockRestApi.Verify(api => api.GetFlagAsync(code), Times.Once);
        }

        [Fact]
        public async Task GetFlag_ReturnsApiData()
        {
            var data = _fixture.Create<byte[]>();
            _mockRestApi
                .Setup(api => api.GetFlagAsync(It.IsAny<string>()))
                .ReturnsAsync(data);

            // Act
            var result = await _sut.GetFlagAsync(_fixture.Create<string>());

            // Assert
            result.Should().BeEquivalentTo(data);
        }
    }
}