using Finance.Data;
using Finance.Domain.Prices;
using Finance.Utils;

namespace Finance.Domain.Yahoo;

public class YahooDataManager
{
    private readonly IWebDataClient _webClient;
    private readonly IFileDataClient _fileClient;
    private readonly IFileIO _fileIO;

    private Func<PriceSet, string, DateTime[], long, long, bool, Task> _historyDataDelegate;
    private Func<PriceSet, string, DateTime[], long, long, bool, Task> _chartDataDelegate;

    public YahooDataManager(IWebDataClient webClient, IFileDataClient fileClient, IFileIO fileIO)
    {
        _webClient = webClient ?? throw new NullReferenceException(nameof(webClient));
        _fileClient = fileClient ?? throw new NullReferenceException(nameof(fileClient));
        _fileIO = fileIO ?? throw new NullReferenceException(nameof(fileIO));

        _historyDataDelegate = async (PriceSet prices, string stock, DateTime[] dates, long start, long end, bool writeRawData) =>
        {
            var response = await _webClient.GetYahooHistoryData(stock, start, end, writeRawData);            
            prices = prices.AddPrices(response.ToPriceSet(dates, stock.HandleSuffix()), stock.HandleSuffix());
        };

        _chartDataDelegate = async (PriceSet prices, string stock, DateTime[] dates, long start, long end, bool writeRawData) =>
        {
            var response = await _webClient.GetYahooChartData(stock, start, end, writeRawData);
            prices = prices.AddPrices(response.chart.result.Single().ToPriceSet(dates), stock.HandleSuffix());
        };
    }

    public async Task GeneratePriceHistoryDataFromApi(DateTime[] dates, string[] stocks, bool writeRawData = false)
    {
        await GeneratePriceDataFromApi(_historyDataDelegate, dates, stocks, writeRawData);
    }

    public async Task GeneratePriceChartDataFromApi(DateTime[] dates, string[] stocks, bool writeRawData = false)
    {
        await GeneratePriceDataFromApi(_chartDataDelegate, dates, stocks, writeRawData);
    }

    public void GeneratePriceHistoryDataFromFile(string fileName, string stock)
    {        
        _fileIO.WriteText(_fileClient.GetYahooFileHistoryData(fileName)
            .ToPriceSet(new DateTime[0], stock)
            .Interpolate(new [] { stock })
            .ConvertToCsv(QuoteKeys.Headers), 
                FileNameTemplate($"Prices_{stock}", ".csv"));
    }

    public void GeneratePriceChartDataFromFile(string fileName, string stock)
    {        
        _fileIO.WriteText(_fileClient.GetYahooFileChartData(fileName).chart.result.Single()
            .ToPriceSet(new DateTime[0])
            .Interpolate(new [] { stock })
            .ConvertToCsv(QuoteKeys.Headers), 
                FileNameTemplate($"Prices_{stock}", ".csv"));
    }

    private async Task GeneratePriceDataFromApi(Func<PriceSet, string, DateTime[], long, long, bool, Task> dataDelegate, DateTime[] dates, string[] stocks, bool writeRawData = false)
    {
        var start = (long)dates.OrderBy(d => d).First().ToUnixTimeStamp();
        var end = (long)dates.OrderBy(d => d).Last().ToUnixTimeStamp();
        var allPrices = new PriceSet();
        foreach(var stock in stocks)
            await dataDelegate.Invoke(allPrices, stock, dates, start, end, writeRawData);

        var trimmedStocks = stocks.Select(s => s.HandleSuffix()).ToArray();
        
        _fileIO.WriteText(allPrices
            .Interpolate(stocks)
            .ConvertToCsv(QuoteKeys.Headers), 
                FileNameTemplate($"Prices_{string.Join("_", trimmedStocks)}", ".csv"));
    }

    private static string FileNameTemplate(string datatype, string extension) =>
        $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}_{datatype}{extension}";
}
