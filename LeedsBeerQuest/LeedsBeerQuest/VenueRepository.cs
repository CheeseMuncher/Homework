namespace LeedsBeerQuest;

public interface IVenueRepository
{
    HashSet<string> GetAllTags();

    HashSet<VenueDefinition> GetVenues(VenueQuery query);
}

public class VenueRepository
{
    private readonly IVenueRawData _venueRawData;

    public VenueRepository(IVenueRawData venueRawData)
    {
        _venueRawData = venueRawData;
    }

    public HashSet<string> GetAllTags() => _venueRawData.GetAllVenues()?.SelectMany(v => v.Tags)?.ToHashSet() ?? new HashSet<string>();

    public HashSet<VenueDefinition> GetVenues(VenueQuery query)
    {
        var predicate = GetPredicate(query);
        return _venueRawData.GetAllVenues()?.Where(vd => predicate(vd))?.ToHashSet() ?? new HashSet<VenueDefinition>();
    }

    private Func<VenueDefinition, bool> GetPredicate(VenueQuery query)
    {
        return (VenueDefinition vd) => 
            query.Tags.All(tag => vd.Tags.Contains(tag))
            && query.MinimumBeerStars <= vd.BeerStars
            && query.MinimumAtmosphereStars <= vd.AtmosphereStars
            && query.MinimumAmenitiesStars <= vd.AmenitiesStars
            && query.MinimumValueStars <= vd.ValueStars
            && (query.IncludeClosedVenues ? true : vd.VenueCategory != VenueCategory.ClosedVenues);
    }
}
