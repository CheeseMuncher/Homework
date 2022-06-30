using Finance.Domain.Yahoo;
using Finance.Domain.TraderMade;

namespace Finance.Data;

public class HttpRequestFactory : IHttpRequestFactory 
{
    public HttpRequestMessage GetHistoryDataTraderMadeRequest(string currencyPair, string date) =>
        new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{GetTraderMadeUri}{GetTraderMadeQueryString(currencyPair, date)}"),
        };

    public HttpRequestMessage GetHistoryDataYahooRequest(string stock, long start, long end) =>
        new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{GetHistoryDataUri}{GetHistoryDataQueryString(stock, start, end)}"),
            Headers = 
            {
                { YahooConstants.RapidApiHeaderKey, FinanceApiCredentials.RapidApiHeaderValue },
                { YahooConstants.HostHeaderKey, YahooConstants.HostHeaderValue }
            }
        };

    public HttpRequestMessage GetChartDataYahooRequest(string stock, long start, long end) =>
        new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{GetChartDataUri}{GetChartDataQueryString(stock, start, end)}"),
            Headers = 
            {
                { YahooConstants.RapidApiHeaderKey, FinanceApiCredentials.RapidApiHeaderValue },
                { YahooConstants.HostHeaderKey, YahooConstants.HostHeaderValue }
            }
        };

    private string GetTraderMadeUri =>
        $"{TraderMadeConstants.BasePath}{TraderMadeConstants.Endpoints.GetHistoricalData}";
    
    private string GetTraderMadeQueryString(string currencyPair, string date) =>
        $"?currency={currencyPair}&date={date}&api_key={FinanceApiCredentials.TraderMadeApiKey}";

    private string GetHistoryDataUri =>
        $"{YahooConstants.BasePath}{YahooConstants.Endpoints.GetHistoryData}";
    
    private string GetHistoryDataQueryString(string stock, long start, long end) =>
        $"?symbol={stock}&from={start}&api_key={end}&events=div&interval=1d&region=GB";

    private string GetChartDataUri =>
        $"{YahooConstants.BasePath}{YahooConstants.Endpoints.GetChart}";
    
    private string GetChartDataQueryString(string stock, long start, long end) =>
        $"?symbol={stock}&period1={start}&period2={end}&range=10y&interval=1d&region=GB";
}

public interface IHttpRequestFactory
{
    HttpRequestMessage GetHistoryDataTraderMadeRequest(string currencyPair, string date);
    HttpRequestMessage GetHistoryDataYahooRequest(string stock, long start, long end);
    HttpRequestMessage GetChartDataYahooRequest(string stock, long start, long end);
}