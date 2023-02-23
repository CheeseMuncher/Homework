namespace LeedsBeerQuestTests;

public class VenueTests
{
    [Fact]
    public void SetDistance_CalculatesDistanceCorrectly()
    {
        var venue = new VenueDefinition { Latitude = 53.801592839686556m, Longitude = -1.5556780429376729m };
        var query = new VenueQuery();

        var result = new Venue(venue);
        result.SetDistance(query.Latitude, query.Longitude);

        result.DistanceMetres.Should().Be(48);
    }
}