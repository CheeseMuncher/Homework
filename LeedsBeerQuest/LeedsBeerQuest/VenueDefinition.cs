using Geolocation;

namespace LeedsBeerQuest;

public class VenueDefinition
{
    public string Name { get; set; }
    public VenueCategory VenueCategory { get; set; }
    public string Url { get; set; }
    public DateTime Date { get; set; }
    public string Excerpt { get; set; }
    public string Thumbnail { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Twitter { get; set; }
    public decimal BeerStars { get; set; }
    public decimal AtmosphereStars { get; set; }
    public decimal AmenitiesStars { get; set; }
    public decimal ValueStars { get; set; }
    public HashSet<string> Tags { get; set; } = new HashSet<string>();
}

public class Venue : VenueDefinition
{
    public Venue() { }
    public Venue(VenueDefinition definition)
    {
        this.Name = definition.Name;
        this.VenueCategory = definition.VenueCategory;
        this.Url = definition.Url;
        this.Date = definition.Date;
        this.Excerpt = definition.Excerpt;
        this.Thumbnail = definition.Thumbnail;
        this.Latitude = definition.Latitude;
        this.Longitude = definition.Longitude;
        this.Address = definition.Address;
        this.Phone = definition.Phone;
        this.Twitter = definition.Twitter;
        this.BeerStars = definition.BeerStars;
        this.AtmosphereStars = definition.AtmosphereStars;
        this.AmenitiesStars = definition.AmenitiesStars;
        this.ValueStars = definition.ValueStars;
        this.Tags = definition.Tags;
    }
    public int DistanceMetres { get; set; }
    public void SetDistance(decimal lat, decimal lng) 
        => DistanceMetres = (int)(GeoCalculator.GetDistance((double)lat, (double)lng, (double)this.Latitude, (double)this.Longitude, 6) * 1609.344);
}

public enum VenueCategory
{
    BarReviews = 0,
    ClosedVenues = 1,
    OtherReviews = 2,
    PubReviews = 3,
    Uncategorized = 4
}
