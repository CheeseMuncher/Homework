namespace Finance.Data;

public class HttpRequestFactory : IHttpRequestFactory 
{
    public HttpRequestMessage GetYahooFinanceRequest(string stock, double start, double end) =>
        new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{YahooFinanceApiCredentials.BasePath}{GetGetYahooFinanceQueryString(stock, start, end)}"),
            Headers = 
            {
                 { YahooFinanceApiCredentials.HeaderKeyKey, YahooFinanceApiCredentials.HeaderKeyValue },
                 { YahooFinanceApiCredentials.HeaderHostKey, YahooFinanceApiCredentials.HeaderHostValue }
            }
        };

    private string GetGetYahooFinanceQueryString(string stock, double start, double end) =>
        $"?symbol={stock}&from={start}&to={end}&events=div&interval=1d&region=UK";
}

public interface IHttpRequestFactory
{
    HttpRequestMessage GetYahooFinanceRequest(string stock, double start, double end);
}