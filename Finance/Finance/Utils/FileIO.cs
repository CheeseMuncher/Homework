namespace Finance.Utils;

public interface IFileIO
{
    bool DataFileExists(string fileName);
    bool SecretsFileExists(string fileName);
    IEnumerable<string> ReadLinesFromFile(string fileName);
    void WriteText(string text, string fileName);
}

public class FileIO : IFileIO
{
    public bool DataFileExists(string fileName) => File.Exists(GetDataPath(fileName));
    public bool SecretsFileExists(string fileName) => File.Exists(GetSecretsPath(fileName));
    public IEnumerable<string> ReadLinesFromFile(string fileName)
    {
        if (!DataFileExists(fileName))
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
    private static string GetSecretsPath(string fileName) => Path.Combine("Secrets", fileName);
}