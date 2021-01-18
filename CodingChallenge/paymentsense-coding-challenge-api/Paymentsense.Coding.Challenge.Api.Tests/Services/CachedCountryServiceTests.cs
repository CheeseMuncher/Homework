using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Paymentsense.Coding.Challenge.Api.Models;
using Paymentsense.Coding.Challenge.Api.Services;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Paymentsense.Coding.Challenge.Api.Tests.Services
{
    internal delegate void OutDelegate<TIn, TOut>(TIn input, out TOut output);

    public class CachedCountryServiceTests
    {
        private readonly Mock<IRestCountriesApi> _mockRestApi = new Mock<IRestCountriesApi>();
        private readonly Mock<IMemoryCache> _mockCache = new Mock<IMemoryCache>();
        private readonly ICountryService _sut;
        private object whatever;

        public CachedCountryServiceTests()
        {
            _mockCache
                .Setup(mc => mc.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());

            _sut = new CachedCountryService(_mockRestApi.Object, _mockCache.Object);
        }

        [Fact]
        public async Task GetAllCountriesAsync_InvokesCache()
        {
            // Act
            await _sut.GetAllCountriesAsync();

            // Assert
            _mockCache.Verify(mc => mc.TryGetValue("Countries", out whatever), Times.Once);
        }

        [Fact]
        public async Task GetAllCountriesAsync_InvokesApi_IfCacheMiss()
        {
            // Arrange
            _mockCache
                .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out whatever))
                .Returns(false);

            // Act
            await _sut.GetAllCountriesAsync();

            // Assert
            _mockRestApi.Verify(api => api.GetAllCountriesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllCountriesAsync_DoesNotInvokesApi_IfCacheHit()
        {
            // Arrange
            _mockCache
                .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out whatever))
                .Returns(true);

            // Act
            await _sut.GetAllCountriesAsync();

            // Assert
            _mockRestApi.Verify(api => api.GetAllCountriesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetAllCountriesAsync_ReturnsCacheData_IfCacheHit()
        {
            // Arrange
            var data = new[] { new Country { Name = "China" } };
            _mockCache
                .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out whatever))
                .Callback(new OutDelegate<object, object>((object k, out object v) => v = data))
                .Returns(true);

            // Act
            var result = await _sut.GetAllCountriesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.Single().Name.Should().Be(data.Single().Name);
        }

        [Fact]
        public async Task GetAllCountriesAsync_WritesApiResultToCache_IfCacheMiss()
        {
            // Arrange
            var response = new[] { new Country { Name = "Greece" } };
            _mockRestApi
                .Setup(api => api.GetAllCountriesAsync())
                .ReturnsAsync(response);

            var cacheHits = 0;
            _mockCache
                .Setup(mc => mc.TryGetValue(It.IsAny<object>(), out whatever))
                .Callback(new OutDelegate<object, object>((object k, out object v) => v = response))
                .Returns(() => cacheHits++ > 0);

            // Act
            await _sut.GetAllCountriesAsync();
            var result = await _sut.GetAllCountriesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.Single().Name.Should().Be(response.Single().Name);
            _mockRestApi.Verify(api => api.GetAllCountriesAsync(), Times.Once);
        }
    }
}