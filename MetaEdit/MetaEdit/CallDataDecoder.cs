using System;
using System.Collections.Generic;

namespace MetaEdit
{
    public class CallDataDecoder : IFileNameDecoder<CallData>
    {
        public CallData DecodeFileName(string fileName, params string[] paramaters)
        {
            throw new NotImplementedException();
        }

        public void SetConvention(char separator, IEnumerable<ConventionElement> conventions)
        {
            throw new NotImplementedException();
        }

        public bool TryExtractFileName(string path, out string name)
        {
            throw new NotImplementedException();
        }
    }
}