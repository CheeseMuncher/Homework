using Finance.Domain.Yahoo.Models;
using Finance.Utils;
using System.Text;
using System.Text.Json;

namespace Finance.Data;

public class FileDataClient : IFileDataClient
{
    private readonly IFileIO _fileIO;

    public FileDataClient(IFileIO fileIO)
    {
        _fileIO = fileIO ?? throw new ArgumentException(nameof(fileIO));
    }

    public HistoryResponse GetYahooFileHistoryData(string fileName)
    {
        return GetYahooFileData<HistoryResponse>(fileName);
    }

    public ChartResponse GetYahooFileChartData(string fileName)
    {
        return GetYahooFileData<ChartResponse>(fileName);
    }

    private T GetYahooFileData<T>(string fileName) where T : new()
    {
        if (!_fileIO.DataFileExists(fileName))
            return new T();

        var sb = new StringBuilder();
        foreach(var line in _fileIO.ReadLinesFromFile(fileName))
            sb.AppendLine(line);

        return JsonSerializer.Deserialize<T>(sb.ToString());
    }
}

public interface IFileDataClient
{
    HistoryResponse GetYahooFileHistoryData(string fileName);
    ChartResponse GetYahooFileChartData(string fileName);
}