using Finance.Domain.TraderMade.Models;
using Finance.Domain.Yahoo.Models;
using Finance.Utils;
using System.Text;
using System.Text.Json;

namespace Finance.Data;

public class FileDataClient : IFileDataClient
{
    private readonly IFileIO _fileIO;
    private readonly JsonSerializerOptions _jsonOptions;

    public FileDataClient(IFileIO fileIO)
    {
        _fileIO = fileIO ?? throw new ArgumentException(nameof(fileIO));
        _jsonOptions = new JsonSerializerOptions();
        _jsonOptions.Converters.Add(new DateTimeConverter());
    }

    public ForexHistoryResponse GetTraderMadeHistoryData(string fileName)
    {
        return GetFileData<ForexHistoryResponse>(fileName);
    }

    public HistoryResponse GetYahooFileHistoryData(string fileName)
    {
        return GetFileData<HistoryResponse>(fileName);
    }

    public ChartResponse GetYahooFileChartData(string fileName)
    {
        return GetFileData<ChartResponse>(fileName);
    }

    private T GetFileData<T>(string fileName) where T : new()
    {
        if (!_fileIO.FileExists(fileName))
            return new T();

        var sb = new StringBuilder();
        foreach(var line in _fileIO.ReadLinesFromFile(fileName))
            sb.AppendLine(line);

        return JsonSerializer.Deserialize<T>(sb.ToString(), _jsonOptions);
    }
}

public interface IFileDataClient
{
    ForexHistoryResponse GetTraderMadeHistoryData(string fileName);
    HistoryResponse GetYahooFileHistoryData(string fileName);
    ChartResponse GetYahooFileChartData(string fileName);
}