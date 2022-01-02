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
        if (!_fileIO.FileExists(fileName))
            return new HistoryResponse();

        var sb = new StringBuilder();
        foreach(var line in _fileIO.ReadLinesFromFile(fileName))
            sb.AppendLine(line);

        return JsonSerializer.Deserialize<HistoryResponse>(sb.ToString());
    }
}

public interface IFileDataClient
{
    HistoryResponse GetYahooFileHistoryData(string fileName);
}