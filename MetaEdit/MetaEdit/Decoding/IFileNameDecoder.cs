namespace MetaEdit.Decoding
{
    /// <summary>
    /// Tries to build an object using a supplied path & file name, following some convention
    /// </summary>
    public interface IFileNameDecoder<T>
    {
        T DecodeFileName(string fileName, params string[] parameters);
    }
}