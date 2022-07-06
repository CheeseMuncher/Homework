using Finance.Domain.Yahoo;

namespace Finance.Data;

public class HttpRequestFactory : IHttpRequestFactory 
{
    public HttpRequestMessage GetHistoryDataYahooRequest(string stock, long start, long end) =>
        new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{GetHistoryDataUri()}{GetHistoryDataQueryString(stock, start, end)}"),
            Headers = 
            {
                { YahooConstants.RapidApiHeaderKey, YahooFinanceApiCredentials.RapidApiHeaderValue },
                { YahooConstants.HostHeaderKey, YahooConstants.HostHeaderValue }
            }
        };

    public HttpRequestMessage GetChartDataYahooRequest(string stock, long start, long end) =>
        new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{GetChartDataUri()}{GetChartDataQueryString(stock, start, end)}"),
            Headers = 
            {
                { YahooConstants.RapidApiHeaderKey, YahooFinanceApiCredentials.RapidApiHeaderValue },
                { YahooConstants.HostHeaderKey, YahooConstants.HostHeaderValue }
            }
        };

    private string GetHistoryDataUri() =>
        $"{YahooConstants.BasePath}{YahooConstants.Endpoints.GetHistoryData}";
    
    private string GetHistoryDataQueryString(string stock, long start, long end) =>
        $"?symbol={stock}&from={start}&to={end}&events=div&interval=1d&region=GB";

    private string GetChartDataUri() =>
        $"{YahooConstants.BasePath}{YahooConstants.Endpoints.GetChart}";
    
    private string GetChartDataQueryString(string stock, long start, long end) =>
        $"?symbol={stock}&period1={start}&period2={end}&range=10y&interval=1d&region=GB";
}

public interface IHttpRequestFactory
{
    HttpRequestMessage GetHistoryDataYahooRequest(string stock, long start, long end);
    HttpRequestMessage GetChartDataYahooRequest(string stock, long start, long end);
}