using Finance.Domain.Yahoo;
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

    public async Task<Response> GetYahooHistoricalData(string stock, long start, long end, bool writeRawData = false)
    {
        var request = _requestFactory.GetHistoricalDataYahooRequest(stock, start, end);
        using (var response = await _client.SendAsync(request))
        {
            if (!response.IsSuccessStatusCode)
                Console.WriteLine($"Error fetching {stock} data from Yahoo API");

            var body = await response.Content.ReadAsStringAsync();
            if (writeRawData)
                _fileIO.WriteText(body, $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}_RawData_{stock}.json");

            return JsonSerializer.Deserialize<Response>(body.ToString());
        }
    }
}

public interface IWebDataClient
{
    Task<Response> GetYahooHistoricalData(string stock, long start, long end, bool writeRawData = false);
}