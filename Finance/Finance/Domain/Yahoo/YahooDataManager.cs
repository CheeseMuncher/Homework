using Finance.Data;
using Finance.Domain.Prices;
using Finance.Utils;

namespace Finance.Domain.Yahoo;

public class YahooDataManager
{
    private readonly IWebDataClient _dataClient;
    private readonly IFileIO _fileIO;

    public YahooDataManager(IWebDataClient dataClient, IFileIO fileIO)
    {
        _dataClient = dataClient ?? throw new NullReferenceException(nameof(dataClient));
        _fileIO = fileIO ?? throw new NullReferenceException(nameof(fileIO));
    }

    public async Task GeneratePriceDataFromApi(DateTime[] dates, string[] stocks, bool writeRawData = false)
    {
        var start = dates.OrderBy(d => d).First().ToUnixTimeStamp();
        var end = dates.OrderBy(d => d).Last().ToUnixTimeStamp();
        var allPrices = new PriceSet();
        foreach(var stock in stocks)
        {
            var response = await _dataClient.GetYahooApiData(stock, start, end, writeRawData);            
            allPrices = allPrices.AddPrices(response.ToPriceSet(dates, stock.HandleIndex()), stock.HandleIndex());
        }
        var trimmedStocks = stocks.Select(s => s.HandleIndex()).ToArray();
        
        _fileIO.WriteText(allPrices
            .Interpolate(trimmedStocks)
            .ConvertToCsv(Constants.Headers), 
                FileNameTemplate($"Prices_{string.Join("_", trimmedStocks)}", ".csv"));
    }

    private static string FileNameTemplate(string datatype, string extension) =>
            $"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")}_{datatype}{extension}";
}
