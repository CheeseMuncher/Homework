namespace Finance.Domain.Yahoo.Models;

public class Price
{
    public long date { get; set; }
    public decimal? open { get; set; } = 0m;
    public decimal? high { get; set; }
    public decimal? low { get; set; }
    public decimal? close { get; set; } = 0m;
    public decimal? volume { get; set; }
    public decimal? adjclose { get; set; }
}