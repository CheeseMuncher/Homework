using Finance.Domain.Prices;

namespace Finance.Utils;

public class FileIO : IFileIO
{
    public bool FileExists(string fileName) => File.Exists(GetDataPath(fileName));

    public IEnumerable<string> ReadLinesFromFile(string fileName)
    {
        if (!FileExists(fileName))
            yield break;                

        string? s;
        using (StreamReader sr = File.OpenText(GetDataPath(fileName)))
            while ((s = sr.ReadLine()) != null)
                yield return s;
    }

    public void WriteText(string text, string fileName)
    {
        using (StreamWriter sw = File.CreateText(GetDataPath(fileName)))
            sw.WriteLine(text);             
    }

    private static string GetDataPath(string fileName) => Path.Combine("FinanceData", fileName);
}

public interface IFileIO
{
    bool FileExists(string fileName);
    IEnumerable<string> ReadLinesFromFile(string fileName);
    void WriteText(string text, string fileName);
}