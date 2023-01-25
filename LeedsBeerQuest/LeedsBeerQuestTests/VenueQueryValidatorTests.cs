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
        var validTag1 = $"{Guid.NewGuid()}";
        var validTag2 = $"{Guid.NewGuid()}";
        _mockRepository.Setup(repo => repo.GetAllTags()).Returns(new HashSet<string> { validTag1, validTag2 });
        var invalidTag = $"{Guid.NewGuid()}";
        var input = new VenueQuery { Tags = new HashSet<string> { invalidTag } };

        var result = _sut.Validate(input);

        result.IsValid.Should().BeFalse();
        var error = result.Errors.Should().ContainSingle().Which;
        error.ErrorMessage.Should().Be($"Tag value {invalidTag} is invalid, valid tags are: {validTag1},{validTag2}");
    }

    [Fact]
    public void Validate_ReturnsValidResult_IfTagDiffersOnlyByCase()
    {
        var tag = $"{Guid.NewGuid()}";
        _mockRepository.Setup(repo => repo.GetAllTags()).Returns(new HashSet<string> { tag.ToLower() });
        var input = new VenueQuery { Tags = new HashSet<string> { tag.ToUpper() } };

        var result = _sut.Validate(input);

        result.IsValid.Should().BeTrue();
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
    public void Validate_MakesSingleInvocation_OnRepository()
    {
        _mockRepository.Setup(repo => repo.GetAllTags()).Returns(new HashSet<string> { $"{Guid.NewGuid()}" });
        var input = new VenueQuery { Tags = new HashSet<string> { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" } };

        var result = _sut.Validate(input);

        _mockRepository.Verify(repo => repo.GetAllTags(), Times.Once);
    }    

    [Fact]
    public void Validate_HandlesNullRepositoryResponse()
    {
         _mockRepository.Setup(repo => repo.GetAllTags()).Returns(null as HashSet<string>);
        var input = new VenueQuery { Tags = new HashSet<string> { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" } };

        var result = _sut.Validate(input);

        _mockRepository.Verify(repo => repo.GetAllTags(), Times.Once);
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
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(invalidTag1));
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains(invalidTag2));
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
        if (property is not null)
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
        if (property is not null)
            property.SetValue(input, 5.000001m);

        var result = _sut.Validate(input);

        result.IsValid.Should().BeFalse();
        var error = result.Errors.Should().ContainSingle().Which;
        error.ErrorMessage.Should().Be($"{propertyName} value {5.000001m} is invalid");
    }
}