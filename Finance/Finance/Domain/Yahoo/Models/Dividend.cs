namespace Finance.Domain.Yahoo.Models;

public class Dividend : DividendPayout
{
    public string stock { get; set; } = null!;
}