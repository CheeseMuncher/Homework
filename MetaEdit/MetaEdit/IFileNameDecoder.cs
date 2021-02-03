using System.Collections.Generic;

namespace MetaEdit
{
    /// <summary>
    /// Tries to build an object using a supplied path & file name, following some convention
    /// </summary>
    public interface IFileNameDecoder<T>
    {
        void SetConvention(char separator, IEnumerable<ConventionElement> conventions);

        bool TryExtractFileName(string path, out string name);

        T DecodeFileName(string fileName, params string[] paramaters);
    }
}