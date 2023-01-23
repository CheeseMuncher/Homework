using System.Collections;

namespace LeedsBeerQuestTests;

public class StarPropertiesClassData : IEnumerable<object[]>
{
    private HashSet<object[]> _starProperties = new HashSet<object[]>
    {
        new object[] { nameof(VenueQuery.MinimumBeerStars) },
        new object[] { nameof(VenueQuery.MinimumAtmosphereStars) },
        new object[] { nameof(VenueQuery.MinimumAmenitiesStars) },
        new object[] { nameof(VenueQuery.MinimumValueStars) }
    };

    public IEnumerator<object[]> GetEnumerator() => _starProperties.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _starProperties.GetEnumerator();
}