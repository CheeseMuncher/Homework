namespace LeedsBeerQuest.Models;

public class VenueDefinition
{
    public string Name { get; set; } = "";
    public VenueType VenueType { get; set; }
    public string Url { get; set; } = "";
    public DateTime Date { get; set; }
    public string Excerpt { get; set; } = "";
    public string Thumbnail { get; set; } = "";
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Twitter { get; set; } = "";
    public decimal BeerStars { get; set; }
    public decimal AtmosphereStars { get; set; }
    public decimal AmenitiesStars { get; set; }
    public decimal ValueStars { get; set; }
    public HashSet<string> Tags { get; set; } = new HashSet<string>();
}