using Finance.Domain.Prices;
using Finance.Utils;

namespace Finance.Domain.Yahoo;

public static class ResponseExtensions
{
    public static PriceSet ToPriceSet(this Response result, DateTime[] allDates, string stock)
    {
        var prices = allDates.ToDictionary(date => date, date => new HashSet<StockPrice>());
        foreach (var price in result.prices)
        {
            var date = price.date.UnixToDateTime().Date;
            if (!prices.ContainsKey(date))
                prices[date] = new HashSet<StockPrice>();

            if (price.close > 0)
                prices[date].Add(new StockPrice { Stock = stock, Price = price.close.RoundToSignificantDigits(6) });
        }
        return new PriceSet { Prices = prices };        
    }
}

