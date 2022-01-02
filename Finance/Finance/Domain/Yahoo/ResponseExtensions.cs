using Finance.Domain.Prices;
using Finance.Domain.Yahoo.Models;
using Finance.Utils;

namespace Finance.Domain.Yahoo;

public static class ResponseExtensions
{
    public static PriceSet ToPriceSet(this HistoryResponse result, DateTime[] allDates, string stock)
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

    public static PriceSet ToPriceSet(this Result result, DateTime[] allDates)
    {
        var prices = allDates.ToDictionary(date => date, date => new HashSet<StockPrice>());
        var dates = result.timestamp?.Select(date => date.UnixToDateTime().Date).ToArray();
        for (int i = 0; i < dates.Length; i++)
        {
            if(!prices.ContainsKey(dates[i]))
                prices[dates[i]] = new HashSet<StockPrice>();

            var price = result.indicators.quote.First().close[i] ?? 0;
            if (price > 0)
            {
                prices[dates[i]].Add(new StockPrice 
                { 
                    Stock = result.meta["symbol"].ToString().HandleSuffix(), 
                    Price = price.RoundToSignificantDigits(6)
                });                    
            }
        }
        return new PriceSet { Prices = prices };
    }    
}