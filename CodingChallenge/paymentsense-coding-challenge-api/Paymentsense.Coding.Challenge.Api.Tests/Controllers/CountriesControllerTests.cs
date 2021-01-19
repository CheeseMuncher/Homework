using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Paymentsense.Coding.Challenge.Api.Controllers;
using Paymentsense.Coding.Challenge.Api.Models;
using Paymentsense.Coding.Challenge.Api.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Paymentsense.Coding.Challenge.Api.Tests.Controllers
{
    public class CountriesControllerTests
    {
        private readonly Fixture _fixture = new Fixture();
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

        [Fact]
        public async Task Get_ReturnsError_IfServiceCallThrows()
        {
            // Arrange
            _mockCountryService
                .Setup(service => service.GetAllCountriesAsync())
                .ThrowsAsync(It.IsAny<Exception>());

            // Act
            var result = await _sut.Get();

            result.Should().BeOfType(typeof(StatusCodeResult));
            (result as StatusCodeResult).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task GetFlag_ReturnsOk_IfServiceCallSuccessful()
        {
            // Arrange
            _mockCountryService
                .Setup(service => service.GetFlagAsync(It.IsAny<string>()))
                .ReturnsAsync(new byte[0]);

            // Act
            var result = await _sut.Get(_fixture.Create<string>());

            // Arrange
            result.Should().BeOfType(typeof(FileContentResult));
            var fileContentResult = result as FileContentResult;
            fileContentResult.ContentType.Should().Be("image/svg+xml");
            fileContentResult.FileContents.Should().NotBeNull();
        }

        [Fact]
        public async Task GetFlag_ReturnsNotFound_IfServiceCallReturnsNull()
        {
            // Arrange
            _mockCountryService
                .Setup(service => service.GetFlagAsync(It.IsAny<string>()))
                .ReturnsAsync(null as byte[]);

            // Act
            var result = await _sut.Get(_fixture.Create<string>());

            // Arrange
            result.Should().BeOfType(typeof(NotFoundResult));
            var notFoundResult = result as NotFoundResult;
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [Fact]
        public async Task GetFlag_ReturnsError_IfServiceCallThrows()
        {
            // Arrange
            _mockCountryService
                .Setup(service => service.GetFlagAsync(It.IsAny<string>()))
                .ThrowsAsync(It.IsAny<Exception>());

            // Act
            var result = await _sut.Get(_fixture.Create<string>());

            result.Should().BeOfType(typeof(StatusCodeResult));
            (result as StatusCodeResult).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}