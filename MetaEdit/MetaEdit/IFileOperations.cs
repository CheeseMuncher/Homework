using System.Collections.Generic;

namespace MetaEdit
{
    public interface IFileOperations
    {
        /// <summary>
        /// Attempts to fetch the files to be processed
        /// </summary>
        string[] GetFiles(string relativePath);

        /// <summary>
        /// Attempts to fetch additional data using the supplied path
        /// </summary>
        IEnumerable<string> GetData(string relativePath);

        /// <summary>
        /// Attempts to extract the file name from the supplied path, with extension
        /// </summary>
        bool TryExtractFileName(string path, out string name);
    }
}