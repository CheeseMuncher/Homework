using System;

namespace MetaEdit
{
    public interface IDecodeConvention
    {
        string[] Separators { get; }

        string[] Convention { get; }

        CallType GetCallType(string input);

        DateTime GetDateTime(string input);
    }
}