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
}