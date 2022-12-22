using Google.Apis.Auth.OAuth2;

namespace Finance.Utils;


public interface IFileIO
{
    bool FileExists(string fileName);
    IEnumerable<string> ReadLinesFromFile(string fileName);
    GoogleCredential BuildCredentialFromFile(string fileName, string[] scopes);
    void WriteText(string text, string fileName);
}

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

    public GoogleCredential BuildCredentialFromFile(string fileName, string[] scopes)
    {
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            return GoogleCredential.FromStream(stream).CreateScoped(scopes);
    }

    public void WriteText(string text, string fileName)
    {
        using (StreamWriter sw = File.CreateText(GetDataPath(fileName)))
            sw.WriteLine(text);             
    }

    private static string GetDataPath(string fileName) => Path.Combine("FinanceData", fileName);
}
