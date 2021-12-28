using Finance.Domain.Yahoo;
using Finance.Utils;
using System.Text;
using System.Text.Json;

namespace Finance.Data;

public class FileDataFetcher
{
    private readonly IFileIO _fileIO;

    public FileDataFetcher(IFileIO fileIO)
    {
        _fileIO = fileIO ?? throw new ArgumentException(nameof(fileIO));
    }

    public Response GetYahooFileData(string fileName)
    {
        if (!_fileIO.FileExists(fileName))
            return new Response();

        var sb = new StringBuilder();
        foreach(var line in _fileIO.ReadLinesFromFile(fileName))
            sb.AppendLine(line);

        return JsonSerializer.Deserialize<Response>(sb.ToString());
    }
}