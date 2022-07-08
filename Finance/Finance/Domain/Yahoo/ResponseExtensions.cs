using Finance.Domain.Prices;
using Finance.Domain.Yahoo.Models;
using Finance.Domain.TraderMade.Models;
using Finance.Utils;

namespace Finance.Domain.Yahoo;

public static class ResponseExtensions
{
    public static PriceSet ToPriceSet(this HashSet<ForexHistoryResponse> responses, DateTime[] allDates, string pair)
    {
        var prices = allDates.ToDictionary(date => date, date => new HashSet<StockPrice>());
        foreach (var response in responses)
        {
            if (!prices.ContainsKey(response.date))
                prices[response.date] = new HashSet<StockPrice>();

            var quote = response.quotes.Single();

            if (quote.close > 0)
                prices[response.date].Add(new StockPrice { Stock = pair, Price = quote.close.RoundToSignificantDigits(6) });
        }
        return new PriceSet { Prices = prices };
    }

    public static PriceSet ToPriceSet(this HistoryResponse response, DateTime[] allDates, string stock)
    {
        var prices = allDates.ToDictionary(date => date, date => new HashSet<StockPrice>());
        foreach (var price in response.prices)
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
        var dates = result.timestamp?.Select(date => date.UnixToDateTime().Date).ToArray();
        if ((allDates ?? new DateTime[0]).Count() == 0 )
            allDates = Reference.GetMarketDays(dates.First(), dates.Last());

        var prices = allDates.ToDictionary(date => date, date => new HashSet<StockPrice>());
        for (int i = 0; i < dates.Length; i++)
        {
            if(!prices.ContainsKey(dates[i]))
                prices[dates[i]] = new HashSet<StockPrice>();

            var price = result.indicators.quote.First().close[i];
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