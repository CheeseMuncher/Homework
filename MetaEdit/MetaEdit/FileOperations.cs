using MetaEdit.Conventions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MetaEdit
{
    public class FileOperations : IFileOperations
    {
        private readonly IDecodeConvention _convention;

        public FileOperations(IDecodeConvention convention)
        {
            _convention = convention;
        }

        public string[] GetFiles(string relativePath)
        {
            return _convention.Extensions.SelectMany(ext => Directory
                .GetFiles(relativePath)
                .Where(file => file.EndsWith(ext)))
                .ToArray();
        }

        public IEnumerable<string> GetData(string relativePath)
        {
            using (var reader = new StreamReader(relativePath))
                while (!reader.EndOfStream)
                    yield return $"{reader.ReadLine()}";
        }

        public bool TryExtractFileName(string path, out string name)
        {
            name = null;
            if (string.IsNullOrEmpty(path) || !PathInExpectedFormat(path))
                return false;

            name = path.Split(Path.DirectorySeparatorChar.ToString()).Last();
            return true;
        }

        private bool PathInExpectedFormat(string path)
        {
            var file = Path.GetFileName(path);
            return !string.IsNullOrEmpty(file)
                && _convention.Extensions.Any(ext => file.EndsWith(ext));
        }
    }
}