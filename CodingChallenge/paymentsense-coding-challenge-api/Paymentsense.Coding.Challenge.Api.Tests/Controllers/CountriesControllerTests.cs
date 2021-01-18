using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Paymentsense.Coding.Challenge.Api.Controllers;
using Paymentsense.Coding.Challenge.Api.Models;
using Paymentsense.Coding.Challenge.Api.Services;
using System.Threading.Tasks;
using Xunit;

namespace Paymentsense.Coding.Challenge.Api.Tests.Controllers
{
    public class CountriesControllerTests
    {
        private readonly Mock<ICountryService> _mockCountryService = new Mock<ICountryService>();
        private readonly CountriesController _sut;

        public CountriesControllerTests()
        {
            _mockCountryService
                .Setup(service => service.GetAllCountriesAsync())
                .ReturnsAsync(new[] { new Country { Name = "testing" } });
            _sut = new CountriesController(_mockCountryService.Object);
        }

        [Fact]
        public async Task Get_ReturnsOk_IfServiceCallSuccessful()
        {
            // Act
            var response = await _sut.Get();

            // Assert
            var result = response as OkObjectResult;
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<Country[]>();
            (result.Value as Country[]).Should().NotBeEmpty();
        }
    }
}