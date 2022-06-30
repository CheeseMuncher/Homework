using Finance.Domain.Yahoo.Models;
using Finance.Domain.TraderMade.Models;
using Finance.Utils;
using System.Text.Json;

namespace Finance.Data;

public class WebDataClient : IWebDataClient
{
    private readonly IFileIO _fileIO;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IHttpRequestFactory _requestFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public WebDataClient(IFileIO fileIO, IHttpClientFactory clientFactory, IHttpRequestFactory requestFactory)
    {
        _fileIO = fileIO ?? throw new ArgumentException(nameof(fileIO));
        _clientFactory = clientFactory ?? throw new ArgumentException(nameof(clientFactory));
        _requestFactory = requestFactory ?? throw new ArgumentException(nameof(requestFactory));
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.Converters.Add(new DateTimeConverter());
    }

    public async IAsyncEnumerable<ForexHistoryResponse> GetTraderMadeHistoryData(string currencyPair, IEnumerable<DateTime> dates)
    {

        foreach (var date in dates)
        {
            var isoDate = date.ToString("yyyy-MM-dd");
            var request = _requestFactory.GetHistoryDataTraderMadeRequest(currencyPair, isoDate);

            using (var response = await _clientFactory.CreateClient(isoDate).SendAsync(request, default(CancellationToken)))
                if (response.IsSuccessStatusCode)
                    yield return JsonSerializer.Deserialize<ForexHistoryResponse>(await response.Content.ReadAsStringAsync(), _jsonOptions);
        }        
    }

    public async Task<HistoryResponse> GetYahooHistoryData(string stock, long start, long end, bool writeRawData = false)
    {
        var request = _requestFactory.GetHistoryDataYahooRequest(stock, start, end);
        return await GetYahooData<HistoryResponse>(request, stock, writeRawData);
    }

    public async Task<ChartResponse> GetYahooChartData(string stock, long start, long end, bool writeRawData = false)
    {
        var request = _requestFactory.GetChartDataYahooRequest(stock, start, end);
        return await GetYahooData<ChartResponse>(request, stock, writeRawData);
    }

    private async Task<T> GetYahooData<T>(HttpRequestMessage request, string stock, bool writeRawData) where T : new()
    {
        using (var response = await _clientFactory.CreateClient(stock).SendAsync(request))
        {
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching {stock} data from Yahoo API");
                return new T();
            }

            var body = await response.Content.ReadAsStringAsync();
            if (writeRawData)
                _fileIO.WriteText(body, $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}_{typeof(T).Name}_{stock}.json");

            return JsonSerializer.Deserialize<T>(body.ToString(), _jsonOptions);
        }
    }
}

public interface IWebDataClient
{
    IAsyncEnumerable<ForexHistoryResponse> GetTraderMadeHistoryData(string currencyPair, IEnumerable<DateTime> dates);
    Task<HistoryResponse> GetYahooHistoryData(string stock, long start, long end, bool writeRawData = false);
    Task<ChartResponse> GetYahooChartData(string stock, long start, long end, bool writeRawData = false);
}