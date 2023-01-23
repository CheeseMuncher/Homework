namespace LeedsBeerQuest;

public interface IVenueRepository
{
    HashSet<string> GetAllTags();
}

public class VenueRepository
{
    private readonly IVenueRawData _venueRawData;

    public VenueRepository(IVenueRawData venueRawData)
    {
        _venueRawData = venueRawData;
    }

    public HashSet<string> GetAllTags() => _venueRawData.GetAllVenues()?.SelectMany(v => v.Tags)?.ToHashSet() ?? new HashSet<string>();
}
