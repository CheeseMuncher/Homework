using Finance.Data;
using Finance.Domain.Prices;
using Finance.Domain.TraderMade.Models;
using Finance.Utils;
using System.Text.Json;

namespace Finance.Domain.Yahoo;

public class FinanceDataManager
{
    private readonly IWebDataClient _webClient;
    private readonly IFileDataClient _fileClient;
    private readonly IFileIO _fileIO;

    private Func<PriceSet, string, DateTime[], long, long, bool, Task> _historyDataDelegate;
    private Func<PriceSet, string, DateTime[], long, long, bool, Task> _chartDataDelegate;
    private readonly JsonSerializerOptions _jsonOptions;

    public FinanceDataManager(IWebDataClient webClient, IFileDataClient fileClient, IFileIO fileIO)
    {
        _webClient = webClient ?? throw new NullReferenceException(nameof(webClient));
        _fileClient = fileClient ?? throw new NullReferenceException(nameof(fileClient));
        _fileIO = fileIO ?? throw new NullReferenceException(nameof(fileIO));
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.Converters.Add(new DateTimeConverter());

        _historyDataDelegate = async (PriceSet prices, string stock, DateTime[] dates, long start, long end, bool writeRawData) =>
        {
            Console.WriteLine($"Fetching history data for {stock} from Yahoo");
            var response = await _webClient.GetYahooHistoryData(stock, start, end, writeRawData);
            prices = prices.AddPrices(response.ToPriceSet(dates, stock.HandleSuffix()), stock.HandleSuffix());
        };

        _chartDataDelegate = async (PriceSet prices, string stock, DateTime[] dates, long start, long end, bool writeRawData) =>
        {
            Console.WriteLine($"Fetching chart data for {stock} from Yahoo");
            var response = await _webClient.GetYahooChartData(stock, start, end, writeRawData);
            prices = prices.AddPrices(response.chart.result.Single().ToPriceSet(dates), stock.HandleSuffix());
        };
    }

    public async Task GenerateForexHistoryDataFromApi(DateTime[] dates, string pair, bool writeRawData = false)
    {
        if (dates.Count() > 1000)
            Console.WriteLine("TraderMate Free Account API Limit is 1000 calls per month, input will be truncated");

        var responses = new HashSet<ForexHistoryResponse>();
        try
        {
            await foreach (var response in _webClient.GetTraderMadeHistoryData(pair, dates.Take(1000)))
                responses.Add(response);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Exception fetching TraderMate data: {ex.Message}");
        }
        finally
        {
            if (writeRawData)
                _fileIO.WriteText(JsonSerializer.Serialize(responses, _jsonOptions), $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}_{pair}.json");
        }

        _fileIO.WriteText(responses
            .ToPriceSet(dates, pair)
            .Interpolate(new[] { pair })
            .ConvertToCsv(QuoteKeys.Headers),
                FileNameTemplate($"Forex_{pair}", ".csv"));
    }

    public async Task GeneratePriceHistoryDataFromApi(DateTime[] dates, string[] stocks, bool writeRawData = false)
    {
        await GeneratePriceDataFromApi(_historyDataDelegate, dates, stocks, writeRawData);
    }

    public async Task GeneratePriceChartDataFromApi(DateTime[] dates, string[] stocks, bool writeRawData = false)
    {
        await GeneratePriceDataFromApi(_chartDataDelegate, dates, stocks, writeRawData);
    }

    public void GenerateForexHistoryDataFromFile(string fileName, string pair)
    {
        _fileIO.WriteText(_fileClient.GetTraderMadeHistoryData(fileName)
            .ToPriceSet(new DateTime[0], pair)
            .Interpolate(new[] { pair })
            .ConvertToCsv(QuoteKeys.Headers),
                FileNameTemplate($"Forex_{pair}", ".csv"));
    }

    public void GeneratePriceHistoryDataFromFile(string fileName, string stock)
    {
        _fileIO.WriteText(_fileClient.GetYahooFileHistoryData(fileName)
            .ToPriceSet(new DateTime[0], stock)
            .Interpolate(new[] { stock })
            .ConvertToCsv(QuoteKeys.Headers),
                FileNameTemplate($"Prices_{stock}", ".csv"));
    }

    public void GeneratePriceChartDataFromFile(string fileName, string stock)
    {
        _fileIO.WriteText(_fileClient.GetYahooFileChartData(fileName).chart.result.Single()
            .ToPriceSet(new DateTime[0])
            .Interpolate(new[] { stock })
            .ConvertToCsv(QuoteKeys.Headers),
                FileNameTemplate($"Prices_{stock}", ".csv"));
    }

    private async Task GeneratePriceDataFromApi(Func<PriceSet, string, DateTime[], long, long, bool, Task> dataDelegate, DateTime[] dates, string[] stocks, bool writeRawData = false)
    {
        var start = (long)dates.OrderBy(d => d).First().ToUnixTimeStamp();
        var end = (long)dates.OrderBy(d => d).Last().ToUnixTimeStamp();
        var allPrices = new PriceSet();
        foreach (var stock in stocks)
            await dataDelegate.Invoke(allPrices, stock, dates, start, end, writeRawData);

        var trimmedStocks = stocks.Select(s => s.HandleSuffix()).ToArray();

        _fileIO.WriteText(allPrices
            .Interpolate(trimmedStocks)
            .ConvertToCsv(QuoteKeys.Headers),
                FileNameTemplate($"Prices_{string.Join("_", trimmedStocks)}", ".csv"));
    }

    private static string FileNameTemplate(string datatype, string extension) =>
        $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}_{datatype}{extension}";
}
