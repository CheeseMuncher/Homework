using Finance.Data;
using Finance.Domain.Yahoo;
using Finance.Utils;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finance; 

class Program
{
    private static readonly IFileIO _fileIO = new FileIO();
    private static readonly HttpClient _client = new HttpClient();
    private static readonly IHttpRequestFactory _requestFactory = new HttpRequestFactory();
    private static readonly IWebDataClient _webClient;
    private static readonly IFileDataClient _fileClient;
    private static readonly YahooDataManager _yahooDataManager;

    static Program()
    {
        _webClient = new WebDataClient(_fileIO, _client, _requestFactory);
        _fileClient = new FileDataClient(_fileIO);
        _yahooDataManager = new YahooDataManager(_webClient, _fileClient, _fileIO);
    }

    static async Task Main(string[] args)
    {
        Console.WriteLine("Fetching data...");

        //GenerateDataFromFile();
        await GenerateDataFromApi();

        Console.WriteLine("Done");
    }

    static void GenerateDataFromFile()
    {
        var file = "2021-12-31T16:44:28_RawData_0P0000KM22.L.json";
        var stock = "0P0000KM22";
        _yahooDataManager.GeneratePriceHistoryDataFromFile(file, stock);
    }

    static async Task GenerateDataFromApi()
    {
        var startDate = new DateTime(2012,10,31);
        var endDate = new DateTime(2021,12,31);
        var stocks = new string[0]
            .Concat(QuoteKeys.ForexTickers)
            //.Concat(QuoteKeys.VanguardTickers.Keys.ToArray())
            .ToArray();

        var dates = Reference.GetMarketDays(startDate.AddDays(-1), endDate.AddDays(1));

        await _yahooDataManager.GeneratePriceChartDataFromApi(dates, stocks, writeRawData:true);
    }
}
