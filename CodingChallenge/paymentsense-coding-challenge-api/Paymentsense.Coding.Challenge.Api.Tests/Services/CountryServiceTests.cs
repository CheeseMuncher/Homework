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
        private readonly Mock<IRestCountriesApi> _mockRestApi = new Mock<IRestCountriesApi>();
        private readonly ICountryService _sut;

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
            var spain = new Country { Name = "Spain" };
            var italy = new Country { Name = "Italy" };
            var egypt = new Country { Name = "Egypt" };
            var response = new[] { spain, italy, egypt };

            _mockRestApi
                .Setup(api => api.GetAllCountriesAsync())
                .ReturnsAsync(response);

            // Act
            var result = await _sut.GetAllCountriesAsync();

            // Assert
            result.Count().Should().Be(3);
            result.Should().BeEquivalentTo(response);
        }
    }
}