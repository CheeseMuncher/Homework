using Finance.Domain.Yahoo.Models;
using Finance.Utils;
using System.Text.Json;

namespace Finance.Data;

public class WebDataClient : IWebDataClient
{
    private readonly IFileIO _fileIO;
    private readonly HttpClient _client;
    private readonly IHttpRequestFactory _requestFactory;

    public WebDataClient(IFileIO fileIO, HttpClient client, IHttpRequestFactory requestFactory)
    {
        _fileIO = fileIO ?? throw new ArgumentException(nameof(fileIO));
        _client = client ?? throw new ArgumentException(nameof(client));
        _requestFactory = requestFactory ?? throw new ArgumentException(nameof(requestFactory));
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
        using (var response = await _client.SendAsync(request))
        {
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error fetching {stock} data from Yahoo API");
                return new T();
            }

            var body = await response.Content.ReadAsStringAsync();
            if (writeRawData)
                _fileIO.WriteText(body, $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}_{typeof(T).Name}_{stock}.json");

            return JsonSerializer.Deserialize<T>(body.ToString());
        }
    }
}

public interface IWebDataClient
{
    Task<HistoryResponse> GetYahooHistoryData(string stock, long start, long end, bool writeRawData = false);
    Task<ChartResponse> GetYahooChartData(string stock, long start, long end, bool writeRawData = false);
}