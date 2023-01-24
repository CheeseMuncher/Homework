using System.Linq.Dynamic.Core;

namespace LeedsBeerQuest;

public interface IVenueRepository
{
    HashSet<string> GetAllTags();

    VenueDefinition[] GetVenues(VenueQuery query, SortKeyType sortKeyType = SortKeyType.Beer);
}

public class VenueRepository : IVenueRepository
{
    private readonly IVenueRawData _venueRawData;

    public VenueRepository(IVenueRawData venueRawData)
    {
        _venueRawData = venueRawData;
    }

    public HashSet<string> GetAllTags() => _venueRawData.GetAllVenues()?.SelectMany(v => v.Tags)?.ToHashSet() ?? new HashSet<string>();

    public VenueDefinition[] GetVenues(VenueQuery query, SortKeyType sortKeyType = SortKeyType.Beer)
    {
        var predicate = GetPredicate(query);
        var sortExpression = GetSortExpression(sortKeyType);
        return _venueRawData.GetAllVenues()
            ?.Where(vd => predicate(vd))
            ?.AsQueryable()
            ?.OrderBy(sortExpression)
            ?.ToArray() ?? Array.Empty<VenueDefinition>();
    }

    private Func<VenueDefinition, bool> GetPredicate(VenueQuery query)
    {
        return (VenueDefinition vd) => 
            query.Tags.All(tag => vd.Tags.Any(t => string.Equals(t, tag, StringComparison.InvariantCultureIgnoreCase)))
            && query.MinimumBeerStars <= vd.BeerStars
            && query.MinimumAtmosphereStars <= vd.AtmosphereStars
            && query.MinimumAmenitiesStars <= vd.AmenitiesStars
            && query.MinimumValueStars <= vd.ValueStars
            && (query.IncludeClosedVenues ? true : vd.VenueCategory != VenueCategory.ClosedVenues);
    }

    private string GetSortExpression(SortKeyType sortKeyType) =>
        sortKeyType switch 
        {
            SortKeyType.Beer => $"{nameof(VenueDefinition.BeerStars)} DESC",
            SortKeyType.Atmosphere => $"{nameof(VenueDefinition.AtmosphereStars)} DESC",
            SortKeyType.Amenities => $"{nameof(VenueDefinition.AmenitiesStars)} DESC",
            SortKeyType.Value => $"{nameof(VenueDefinition.ValueStars)} DESC",
            SortKeyType.Name => $"{nameof(VenueDefinition.Name)} ASC",
            _ => ""
        };
}