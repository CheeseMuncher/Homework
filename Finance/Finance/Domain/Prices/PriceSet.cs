namespace Finance.Domain.Prices;

public class PriceSet
{
    public Dictionary<DateTime, HashSet<StockPrice>> Prices { get; set; } = new Dictionary<DateTime, HashSet<StockPrice>>();
}