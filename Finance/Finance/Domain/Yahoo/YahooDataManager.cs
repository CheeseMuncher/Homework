using Finance.Data;
using Finance.Domain.Prices;
using Finance.Utils;

namespace Finance.Domain.Yahoo;

public class YahooDataManager
{
    private readonly IWebDataClient _webClient;
    private readonly IFileDataClient _fileClient;
    private readonly IFileIO _fileIO;

    public YahooDataManager(IWebDataClient webClient, IFileDataClient fileClient, IFileIO fileIO)
    {
        _webClient = webClient ?? throw new NullReferenceException(nameof(webClient));
        _fileClient = fileClient ?? throw new NullReferenceException(nameof(fileClient));
        _fileIO = fileIO ?? throw new NullReferenceException(nameof(fileIO));
    }

    public async Task GeneratePriceDataFromApi(DateTime[] dates, string[] stocks, bool writeRawData = false)
    {
        var start = (long)dates.OrderBy(d => d).First().ToUnixTimeStamp();
        var end = (long)dates.OrderBy(d => d).Last().ToUnixTimeStamp();
        var allPrices = new PriceSet();
        foreach(var stock in stocks)
        {
            var response = await _webClient.GetYahooHistoricalData(stock, start, end, writeRawData);            
            allPrices = allPrices.AddPrices(response.ToPriceSet(dates, stock.HandleSuffix()), stock.HandleSuffix());
        }
        var trimmedStocks = stocks.Select(s => s.HandleSuffix()).ToArray();
        
        _fileIO.WriteText(allPrices
            .Interpolate(trimmedStocks)
            .ConvertToCsv(QuoteKeys.Headers), 
                FileNameTemplate($"Prices_{string.Join("_", trimmedStocks)}", ".csv"));
    }

    public void GeneratePriceDataFromFile(string fileName, string stock)
    {        
        _fileIO.WriteText(_fileClient.GetYahooFileData(fileName)
            .ToPriceSet(new DateTime[0], stock)
            .Interpolate(new [] { stock })
            .ConvertToCsv(QuoteKeys.Headers), 
                FileNameTemplate($"Prices_{stock}", ".csv"));
    }

    private static string FileNameTemplate(string datatype, string extension) =>
            $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}_{datatype}{extension}";
}
