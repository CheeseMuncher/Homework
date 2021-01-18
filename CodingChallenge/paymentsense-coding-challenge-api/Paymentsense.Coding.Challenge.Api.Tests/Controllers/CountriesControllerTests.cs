using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paymentsense.Coding.Challenge.Api.Controllers;
using System.Threading.Tasks;
using Xunit;

namespace Paymentsense.Coding.Challenge.Api.Tests.Controllers
{
    public class CountriesControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOk_IfServiceCallSuccessful()
        {
            // Arrange
            var controller = new CountriesController();

            // Act
            var response = await controller.Get();

            // Assert
            var result = response as OkObjectResult;
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Value.Should().NotBeNull();
            result.Value.Should().BeOfType<string[]>();
            (result.Value as string[]).Should().NotBeEmpty();
        }
    }
}