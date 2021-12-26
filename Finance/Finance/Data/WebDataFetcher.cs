using Finance.Domain.Yahoo;
using Finance.Utils;
using System.Text.Json;

namespace Finance.Data;

public class WebDataFetcher
{
    private readonly IFileIO _fileIO;
    private readonly HttpClient _client;
    private readonly IHttpRequestFactory _requestFactory;

    public WebDataFetcher(IFileIO fileIO, HttpClient client, IHttpRequestFactory requestFactory)
    {
        _fileIO = fileIO ?? throw new ArgumentException(nameof(fileIO));
        _client = client ?? throw new ArgumentException(nameof(client));
        _requestFactory = requestFactory ?? throw new ArgumentException(nameof(requestFactory));
    }

    public async Task<Response> GetYahooApiData(string stock, double start, double end, bool writeRawData = false)
    {
        var request = _requestFactory.GetYahooFinanceRequest(stock, start, end);
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