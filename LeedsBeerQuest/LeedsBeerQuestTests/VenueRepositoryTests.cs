namespace LeedsBeerQuestTests;

public class VenueRepositoryTests
{
    private readonly Mock<IVenueRawData> _mockRawDataRepo = new();
    private readonly VenueRepository _sut;

    public VenueRepositoryTests()
    {
        _sut = new VenueRepository(_mockRawDataRepo.Object);
    }

    [Fact]
    public void GetAllTags_ReturnsEmptyHashSet_IfRawDataIsNull()
    {
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(null as HashSet<VenueDefinition>);

        var result = _sut.GetAllTags();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAllTags_ReturnsEmptyHashSet_IfRawDataContainsNoTags()
    {
        var definitions = new HashSet<VenueDefinition>()
        {
            new VenueDefinition(),
            new VenueDefinition()
        };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(definitions);

        var result = _sut.GetAllTags();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAllTags_ReturnsVenueTags_IfRawDataSingleVenue()
    {
        var venue = new VenueDefinition { Tags = new HashSet<string> { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" } };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(new HashSet<VenueDefinition>() { venue });

        var result = _sut.GetAllTags();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(venue.Tags);
    }

    [Fact]
    public void GetAllTags_ReturnsCombinedVenueTags()
    {
        var tag1 = $"{Guid.NewGuid()}";
        var tag2 = $"{Guid.NewGuid()}";
        var tag3 = $"{Guid.NewGuid()}";
        var tag4 = $"{Guid.NewGuid()}";

        var venue1 = new VenueDefinition { Tags = new HashSet<string> { tag1, tag2, tag3 } };
        var venue2 = new VenueDefinition { Tags = new HashSet<string> { tag2, tag3, tag4 } };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(new HashSet<VenueDefinition>() { venue1, venue2 });

        var result = _sut.GetAllTags();

        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().BeEquivalentTo(new HashSet<string> { tag1, tag2, tag3, tag4 } );
    }

    [Fact]
    public void GetVenues_ReturnsEmptyHashSet_IfRawDataIsNull()
    {
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(null as HashSet<VenueDefinition>);

        var result = _sut.GetVenues(new VenueQuery());

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [ClassData(typeof(StarPropertiesClassData))]
    public void GetVenues_FiltersQueryStars(string propertyName)
    {
        var venue1 = new VenueDefinition { BeerStars = 1m, AtmosphereStars = 1m, AmenitiesStars = 1m, ValueStars = 1m };
        var venue2 = new VenueDefinition { BeerStars = 1.5m, AtmosphereStars = 1.5m, AmenitiesStars = 1.5m, ValueStars = 1.5m };
        var venue3 = new VenueDefinition { BeerStars = 2m, AtmosphereStars = 2m, AmenitiesStars = 2m, ValueStars = 2m };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(new HashSet<VenueDefinition>() { venue1, venue2, venue3 });

        var query = new VenueQuery();
        var property = typeof(VenueQuery).GetProperty(propertyName);
        property.SetValue(query, 1.2m);

        var result = _sut.GetVenues(query);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(new HashSet<VenueDefinition>() { venue2, venue3 });
    }

    [Theory]
    [ClassData(typeof(StarPropertiesClassData))]
    public void GetVenues_FiltersQueryStars_InclusiveEdgeCase(string propertyName)
    {
        var venue1 = new VenueDefinition { BeerStars = 1m, AtmosphereStars = 1m, AmenitiesStars = 1m, ValueStars = 1m };
        var venue2 = new VenueDefinition { BeerStars = 1.5m, AtmosphereStars = 1.5m, AmenitiesStars = 1.5m, ValueStars = 1.5m };
        var venue3 = new VenueDefinition { BeerStars = 2m, AtmosphereStars = 2m, AmenitiesStars = 2m, ValueStars = 2m };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(new HashSet<VenueDefinition>() { venue1, venue2, venue3 });

        var query = new VenueQuery();
        var property = typeof(VenueQuery).GetProperty(propertyName);
        property.SetValue(query, 2m);

        var result = _sut.GetVenues(query);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(new HashSet<VenueDefinition>() { venue3 });
    }

    [Fact]
    public void GetVenues_FiltersOnAllTags()
    {
        var tag1 = $"{Guid.NewGuid()}";
        var tag2 = $"{Guid.NewGuid()}";
        var tag3 = $"{Guid.NewGuid()}";
        var tag4 = $"{Guid.NewGuid()}";

        var venue1 = new VenueDefinition { Tags = new HashSet<string> { tag1, tag2 } };
        var venue2 = new VenueDefinition { Tags = new HashSet<string> { tag2, tag3 } };
        var venue3 = new VenueDefinition { Tags = new HashSet<string> { tag3, tag4 } };
        var venue4 = new VenueDefinition { Tags = new HashSet<string> { tag1, tag2, tag3, tag4 } };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(new HashSet<VenueDefinition>() { venue1, venue2, venue3, venue4 });

        var result = _sut.GetVenues(new VenueQuery { Tags = new HashSet<string> { tag2, tag3} });

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(new HashSet<VenueDefinition>() { venue2, venue4 });
    }

    [Fact]
    public void GetVenues_FiltersOnTagsCaseInsensitive()
    {
        var tag = $"{Guid.NewGuid()}".ToUpper();
        var venue = new VenueDefinition { Tags = new HashSet<string> { tag } };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(new HashSet<VenueDefinition>() { venue });

        var result = _sut.GetVenues(new VenueQuery { Tags = new HashSet<string> { tag.ToLower() } });

        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(new HashSet<VenueDefinition>() { venue });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GetVenues_FiltersClosedVenues(bool includeClosedVenues)
    {
        var venue1 = new VenueDefinition { VenueCategory = VenueCategory.BarReviews };
        var venue2 = new VenueDefinition { VenueCategory = VenueCategory.ClosedVenues };
        var venue3 = new VenueDefinition { VenueCategory = VenueCategory.PubReviews };
        var venues = new HashSet<VenueDefinition>() { venue1, venue2, venue3 };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(venues);

        var result = _sut.GetVenues(new VenueQuery { IncludeClosedVenues = includeClosedVenues });

        var expectedVenues = includeClosedVenues ? venues : new HashSet<VenueDefinition>() { venue1, venue3 };
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedVenues.Count);
        result.Should().BeEquivalentTo(expectedVenues);
    }

    [Theory]
    [InlineData(SortKeyType.Beer)]
    [InlineData(SortKeyType.Atmosphere)]
    [InlineData(SortKeyType.Amenities)]
    [InlineData(SortKeyType.Value)]
    public void GetVenues_SortsByStarValues(SortKeyType sortKeyType)
    {
        var venue1 = new VenueDefinition { BeerStars = 1m, AtmosphereStars = 1m, AmenitiesStars = 1m, ValueStars = 1m };
        var venue2 = new VenueDefinition { BeerStars = 1.5m, AtmosphereStars = 1.5m, AmenitiesStars = 1.5m, ValueStars = 1.5m };
        var venue3 = new VenueDefinition { BeerStars = 2m, AtmosphereStars = 2m, AmenitiesStars = 2m, ValueStars = 2m };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(new HashSet<VenueDefinition>() { venue1, venue2, venue3 });

        var result = _sut.GetVenues(new VenueQuery(), sortKeyType);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.First().Should().BeEquivalentTo(venue3);
        result.Last().Should().BeEquivalentTo(venue1);
    }

    [Fact]
    public void GetVenues_SortsByName()
    {
        var venue1 = new VenueDefinition { Name = "B" };
        var venue2 = new VenueDefinition { Name = "C" };
        var venue3 = new VenueDefinition { Name = "A" };
        _mockRawDataRepo.Setup(repo => repo.GetAllVenues()).Returns(new HashSet<VenueDefinition>() { venue1, venue2, venue3 });

        var result = _sut.GetVenues(new VenueQuery(), SortKeyType.Name);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.First().Should().BeEquivalentTo(venue3);
        result.Last().Should().BeEquivalentTo(venue2);
    }    

}