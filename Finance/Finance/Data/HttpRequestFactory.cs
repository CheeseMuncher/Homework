using Finance.Domain.Yahoo;

namespace Finance.Data;

public class HttpRequestFactory : IHttpRequestFactory 
{
    public HttpRequestMessage GetHistoricalDataYahooRequest(string stock, long start, long end) =>
        new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{GetHistoricalDataUri()}{GetHistoricalDataQueryString(stock, start, end)}"),
            Headers = 
            {
                { YahooConstants.RapidApiHeaderKey, YahooFinanceApiCredentials.RapidApiHeaderValue },
                { YahooConstants.HostHeaderKey, YahooConstants.HostHeaderValue }
            }
        };

    private string GetHistoricalDataUri() =>
        $"{YahooConstants.BasePath}{YahooConstants.Endpoints.GetHistoricalData}";
    
    private string GetHistoricalDataQueryString(string stock, long start, long end) =>
        $"?symbol={stock}&from={start}&to={end}&events=div&interval=1d&region=UK";
}

public interface IHttpRequestFactory
{
    HttpRequestMessage GetHistoricalDataYahooRequest(string stock, long start, long end);
}