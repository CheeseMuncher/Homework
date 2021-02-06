namespace MetaEdit
{
    public interface IFileOperations
    {
        string[] GetFiles(string relativePath);

        bool TryExtractFileName(string path, out string name);
    }
}