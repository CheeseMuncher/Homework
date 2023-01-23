namespace LeedsBeerQuestTests;

public class VenueQueryValidatorTests
{
    private readonly Mock<IVenueRepository> _mockRepository = new();
    private readonly VenueQueryValidator _sut;

    public VenueQueryValidatorTests()
    {
        _sut = new VenueQueryValidator(_mockRepository.Object);
    }

    [Fact]
    public void Validate_ReturnsValidResult_IfInputValid()
    {
        var input = new VenueQuery();

        var result = _sut.Validate(input);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ReturnsInvalidResult_IfTagInvalid()
    {
        _mockRepository.Setup(repo => repo.GetAllTags()).Returns(new HashSet<string> { $"{Guid.NewGuid()}" });
        var invalidTag = $"{Guid.NewGuid()}";
        var input = new VenueQuery { Tags = new HashSet<string> { invalidTag } };

        var result = _sut.Validate(input);

        result.IsValid.Should().BeFalse();
        var error = result.Errors.Should().ContainSingle().Which;
        error.ErrorMessage.Should().Be($"Tag value {invalidTag} is invalid");
    }

    [Fact]
    public void Validate_ReturnsValidResult_IfTagsAllValid()
    {
        var validTags = new HashSet<string> { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" };
        _mockRepository.Setup(repo => repo.GetAllTags()).Returns(validTags);
        var input = new VenueQuery { Tags = new HashSet<string> { validTags.First(), validTags.Last() } };

        var result = _sut.Validate(input);

        result.IsValid.Should().BeTrue();
    }    

    [Fact]
    public void Validate_ReturnsInvalidResult_WithOneErrorPerInvalidTag()
    {
        var validTags = new HashSet<string> { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" };
        _mockRepository.Setup(repo => repo.GetAllTags()).Returns(validTags);

        var invalidTag1 = $"{Guid.NewGuid()}";
        var invalidTag2 = $"{Guid.NewGuid()}";
        var input = new VenueQuery { Tags = new HashSet<string> { validTags.First(), validTags.Last(), invalidTag1, invalidTag2 } };

        var result = _sut.Validate(input);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(e => e.ErrorMessage == $"Tag value {invalidTag1} is invalid");
        result.Errors.Should().Contain(e => e.ErrorMessage == $"Tag value {invalidTag2} is invalid");
    }

    [Theory]
    [InlineData(-90.000001)]
    [InlineData(90.000001)]
    public void Validate_ReturnsInvalidResult_IfLatitudeInvalid(decimal latitude)
    {
        var input = new VenueQuery { Latitude = latitude };

        var result = _sut.Validate(input);

        result.IsValid.Should().BeFalse();
        var error = result.Errors.Should().ContainSingle().Which;
        error.ErrorMessage.Should().Be($"Latitude value {latitude} is invalid");
    }

    [Theory]
    [InlineData(-180.000001)]
    [InlineData(180.000001)]
    public void Validate_ReturnsInvalidResult_IfLongitudeInvalid(decimal longitude)
    {
        var input = new VenueQuery { Longitude = longitude };

        var result = _sut.Validate(input);

        result.IsValid.Should().BeFalse();
        var error = result.Errors.Should().ContainSingle().Which;
        error.ErrorMessage.Should().Be($"Longitude value {longitude} is invalid");
    }

    [Theory]
    [ClassData(typeof(StarPropertiesClassData))]
    public void Validate_ReturnsInvalidResult_IfStarValueNegative(string propertyName)
    {
        var input = new VenueQuery();
        var property = typeof(VenueQuery).GetProperty(propertyName);
        property.SetValue(input, -0.000001m);

        var result = _sut.Validate(input);

        result.IsValid.Should().BeFalse();
        var error = result.Errors.Should().ContainSingle().Which;
        error.ErrorMessage.Should().Be($"{propertyName} value {-0.000001m} is invalid");
    }

    [Theory]
    [ClassData(typeof(StarPropertiesClassData))]
    public void Validate_ReturnsInvalidResult_IfStarValueExceedsFive(string propertyName)
    {
        var input = new VenueQuery();
        var property = typeof(VenueQuery).GetProperty(propertyName);
        property.SetValue(input, 5.000001m);

        var result = _sut.Validate(input);

        result.IsValid.Should().BeFalse();
        var error = result.Errors.Should().ContainSingle().Which;
        error.ErrorMessage.Should().Be($"{propertyName} value {5.000001m} is invalid");
    }
}