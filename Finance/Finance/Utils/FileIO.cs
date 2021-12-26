namespace Finance.Utils;

public class FileIO : IFileIO
{
    public void WriteText(string text, string fileName)
    {
        using (StreamWriter sw = File.CreateText(GetDataPath(fileName)))
            sw.WriteLine(text);             
    }

    private static string GetDataPath(string fileName) => Path.Combine("Data", fileName);
}

public interface IFileIO
{
    void WriteText(string text, string fileName);
}