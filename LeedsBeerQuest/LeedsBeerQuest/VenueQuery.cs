namespace LeedsBeerQuest;

public class VenueQuery
{
    public HashSet<string> Tags { get; set; } = new HashSet<string>();
    public decimal? Latitude { get; set; } = 53.801205533361426m;
    public decimal? Longitude { get; set; } = -1.556007954901056m;
    public decimal? MinimumBeerStars { get; set; } = 0m;
    public decimal? MinimumAtmosphereStars { get; set; } = 0m;
    public decimal? MinimumAmenitiesStars { get; set; } = 0m;
    public decimal? MinimumValueStars { get; set; } = 0m;

    public bool IncludeClosedVenues { get; set; } = false;
}
