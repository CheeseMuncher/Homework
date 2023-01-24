using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using LeedsBeerQuest.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace LeedsBeerQuestTests;

public class VenuesControllerTests
{
    private readonly Mock<IVenueRepository> _mockVenueRepository = new();
    private readonly Mock<IValidator<VenueQuery>> _mockQueryValidator = new();
    private VenuesController _sut;

    public VenuesControllerTests()
    {
        _mockQueryValidator.Setup(mvv => mvv.Validate(It.IsAny<VenueQuery>())).Returns(new ValidationResult());
        _sut = new VenuesController(_mockVenueRepository.Object, _mockQueryValidator.Object);        
    }

    [Fact]
    public void Get_InvokesValidator_WithCorrectArgs()
    {
        var query = new VenueQuery();

        _sut.Get(query, $"{Guid.NewGuid()}");

        _mockQueryValidator.Verify(mvv => mvv.Validate(query), Times.Once);
    }

    [Fact]
    public void Get_ReturnsBadRequest_WithMessage_IfQueryInvalid()
    {
        var error1 = new ValidationFailure("property1", "message");
        var error2 = new ValidationFailure("property2", "error");
        var validationResult = new ValidationResult { Errors = new List<ValidationFailure> { error1, error2 } };
        _mockQueryValidator.Setup(mvv => mvv.Validate(It.IsAny<VenueQuery>())).Returns(validationResult);

        var result = _sut.Get(new VenueQuery(), $"{Guid.NewGuid()}");

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        var content = (result as BadRequestObjectResult).Value as string;
        content.Should().NotBeNull();
        content.Should().Contain(error1.PropertyName);
        content.Should().Contain(error1.ErrorMessage);
        content.Should().Contain(error2.PropertyName);
        content.Should().Contain(error2.ErrorMessage);
    }

    [Fact]
    public void Get_InvokesReository_WithCorrectArgs()
    {
        var query = new VenueQuery();

        _sut.Get(query, $"{Guid.NewGuid()}");

        _mockVenueRepository.Verify(mvr => mvr.GetVenues(query), Times.Once);
    }

    [Fact]
    public void Get_ReturnsOk_WithRepositoryData()
    {
        var venues = new HashSet<VenueDefinition>
        {
            new VenueDefinition 
            {
                Address = "Something",
                BeerStars = 2.5m,
                Tags = new HashSet<string> { "A tag" }
            },
            new VenueDefinition 
            {
                Twitter = "Twit",
                AtmosphereStars = 1.5m,
                Tags = new HashSet<string> { "Another" }
            }
        };
        _mockVenueRepository.Setup(mvr => mvr.GetVenues(It.IsAny<VenueQuery>())).Returns(venues);

        var result = _sut.Get(new VenueQuery(), $"{Guid.NewGuid()}");

        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        var content = (result as OkObjectResult).Value as string;
        content.Should().Be(JsonSerializer.Serialize(venues));
    }
}