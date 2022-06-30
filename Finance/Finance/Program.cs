using Finance.Data;
using Finance.Domain.Yahoo;
using Finance.Utils;

namespace Finance; 

class Program
{
    private static readonly IFileIO _fileIO = new FileIO();
    private static readonly IHttpClientFactory _clientFactory = new HttpClientFactoryWrapper();
    private static readonly IHttpRequestFactory _requestFactory = new HttpRequestFactory();
    private static readonly IWebDataClient _webClient;
    private static readonly IFileDataClient _fileClient;
    private static readonly YahooDataManager _yahooDataManager;

    static Program()
    {
        _webClient = new WebDataClient(_fileIO, _clientFactory, _requestFactory);
        _fileClient = new FileDataClient(_fileIO);
        _yahooDataManager = new YahooDataManager(_webClient, _fileClient, _fileIO);
    }

    static async Task Main(string[] args)
    {
        var startDate = new DateTime(2020,07,01);
        var endDate = new DateTime(2022,01, 05);
        var stocks = new string[0]
            //.Concat(QuoteKeys.ForexTickers)
            //.Concat(QuoteKeys.VanguardTickers.Keys.ToArray())
            .Concat(QuoteKeys.LiveIsaStocks)
            .Concat(QuoteKeys.LiveSippStocks)
            .Concat(QuoteKeys.LiveMixedStocks)
            .Concat(new [] { "SGE.L","BBOX.L" })
            .ToArray();

        var file = "2022-06-23T21:52:49_Forex_USDGBP.json";
        var stock = "USDDGBP";
        //var stock = "0P0000KM1Y";


        Console.WriteLine("Fetching data...");

        var dates = GenerateMarketDays(startDate, endDate);
        
        //GenerateHistoryDataFromFile(file, stock);
        //await GenerateHistoryDataFromApi(dates, stocks);
        
        //GenerateChartDataFromFile(file, stock);
        //await GenerateChartDataFromApi(dates, stocks);

        Console.WriteLine("Done");
    }

    static DateTime[] GenerateMarketDays(DateTime start, DateTime end)
    {
        var dates = Reference.GetMarketDays(start.AddDays(-1), end.AddDays(1));
        _fileIO.WriteText(string.Join('\n', dates.Select(d => d.ToString("yyyy-MM-dd"))), "Dates.csv");
        return dates;
    }

    static void GenerateHistoryDataFromFile(string file, string stock) =>
        _yahooDataManager.GeneratePriceHistoryDataFromFile(file, stock);

    static void GenerateChartDataFromFile(string file, string stock) =>
        _yahooDataManager.GeneratePriceChartDataFromFile(file, stock);

    static async Task GenerateHistoryDataFromApi(DateTime[] dates, string[] stocks) =>
        await _yahooDataManager.GeneratePriceHistoryDataFromApi(dates, stocks, writeRawData:true);

    static async Task GenerateChartDataFromApi(DateTime[] dates, string[] stocks) =>
        await _yahooDataManager.GeneratePriceChartDataFromApi(dates, stocks, writeRawData:true);
}
