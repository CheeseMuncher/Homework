using System;
using System.Collections.Generic;

namespace MetaEdit.Conventions
{
    public interface IDecodeConvention
    {
        HashSet<string> Extensions { get; }

        HashSet<string> Separators { get; }

        string[] Convention { get; }

        CallType GetCallType(string input);

        DateTime GetDateTime(string input);
    }
}