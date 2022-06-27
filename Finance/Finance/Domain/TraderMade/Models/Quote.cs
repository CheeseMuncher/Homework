namespace Finance.Domain.TraderMade.Models;

public class Quote
{
    public string base_currency { get; set; }
    public string quote_currency { get; set; }
    public decimal close { get; set; }
    public decimal open { get; set; }
    public decimal high { get; set; }
    public decimal low { get; set; }
}