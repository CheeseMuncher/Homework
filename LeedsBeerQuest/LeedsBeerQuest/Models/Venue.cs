using Geolocation;

namespace LeedsBeerQuest.Models;

public class Venue : VenueDefinition
{
    public Venue() { }
    public Venue(VenueDefinition definition)
    {
        this.Name = definition.Name;
        this.VenueType = definition.VenueType;
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