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

        _sut.Get(query);

        _mockQueryValidator.Verify(mvv => mvv.Validate(query), Times.Once);
    }

    [Fact]
    public void Get_ReturnsBadRequest_WithMessage_IfQueryInvalid()
    {
        var error1 = new ValidationFailure("property1", "message");
        var error2 = new ValidationFailure("property2", "error");
        var validationResult = new ValidationResult { Errors = new List<ValidationFailure> { error1, error2 } };
        _mockQueryValidator.Setup(mvv => mvv.Validate(It.IsAny<VenueQuery>())).Returns(validationResult);

        var result = _sut.Get(new VenueQuery());

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        var content = (result as BadRequestObjectResult)?.Value as string;
        content.Should().NotBeNull();
        content.Should().Contain(error1.PropertyName);
        content.Should().Contain(error1.ErrorMessage);
        content.Should().Contain(error2.PropertyName);
        content.Should().Contain(error2.ErrorMessage);
    }

    [Fact]
    public void Get_ReturnsBadRequest_WithMessage_IfSortKeyInvalid()
    {
        var sortKey = $"{Guid.NewGuid()}";

        var result = _sut.Get(new VenueQuery(), sortKey);

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        var content = (result as BadRequestObjectResult)?.Value as string;
        content.Should().Contain(sortKey);
    }

    [Fact]
    public void Get_CombinesErrorMessages_IfQueryAndSortKeyBothInvalid()
    {
        var error = new ValidationFailure("property1", "message");
        var validationResult = new ValidationResult { Errors = new List<ValidationFailure> { error } };
        _mockQueryValidator.Setup(mvv => mvv.Validate(It.IsAny<VenueQuery>())).Returns(validationResult);
        var sortKey = $"{Guid.NewGuid()}";

        var result = _sut.Get(new VenueQuery(), sortKey);

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        var content = (result as BadRequestObjectResult)?.Value as string;
        content.Should().NotBeNull();
        content.Should().Contain(error.PropertyName);
        content.Should().Contain(error.ErrorMessage);
        content.Should().Contain(sortKey);
        foreach (var sortKeyType in Enum.GetNames<SortKeyType>().ToHashSet())
            content.Should().Contain(sortKeyType);
    }

    [Fact]
    public void Get_InvokesRepository_WithCorrectArgs()
    {
        var query = new VenueQuery();
        var sortKey = SortKeyType.Value;

        _sut.Get(query, $"{sortKey}");

        _mockVenueRepository.Verify(mvr => mvr.GetVenues(query, sortKey), Times.Once);
    }

    [Fact]
    public void Get_ReturnsOk_WithRepositoryData()
    {
        var venues = new []
        {
            new Venue 
            {
                Address = "Something",
                BeerStars = 2.5m,
                Tags = new HashSet<string> { "A tag" }
            },
            new Venue 
            {
                Twitter = "Twit",
                AtmosphereStars = 1.5m,
                Tags = new HashSet<string> { "Another" },
                DistanceMetres = 3
            }
        };
        _mockVenueRepository.Setup(mvr => mvr.GetVenues(It.IsAny<VenueQuery>(), It.IsAny<SortKeyType>())).Returns(venues);

        var result = _sut.Get(new VenueQuery());

        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        var content = (result as OkObjectResult)?.Value as string;
        content.Should().Be(JsonSerializer.Serialize(venues));
    }
}