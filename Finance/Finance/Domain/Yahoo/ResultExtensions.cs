using Finance.Domain.Prices;
using Finance.Utils;

namespace Finance.Domain.Yahoo;

public static class ResultExtensions
{
    public static PriceSet ToPriceSet(this Result result, DateTime[] allDates)
    {
        var prices = allDates.ToDictionary(date => date, date => new HashSet<StockPrice>());
        var dates = result.timestamp?.Select(date => date.UnixToDateTime().Date).ToArray();
        for (int i = 0; i < dates.Length; i++)
        {
            if(!prices.ContainsKey(dates[i]))
                prices[dates[i]] = new HashSet<StockPrice>();

            var price = result.indicators.quote.First().close[i] ?? 0;
            prices[dates[i]].Add(new StockPrice 
            { 
                Stock = result.meta["symbol"].ToString(), 
                Price = price.RoundToSignificantDigits(6)
            });                    
        }
        return new PriceSet { Prices = prices };
    }
}

