using System;
using System.Collections.Generic;

namespace MetaEdit.Conventions
{
    public class AllCallRecorderConvention : IDecodeConvention
    {
        public HashSet<string> Extensions { get; } = new HashSet<string> { ".3gp" };

        public HashSet<string> Separators { get; } = new HashSet<string> { "o" };

        public string[] Convention { get; } = new[]
        {
            nameof(CallData.CallTime),
            nameof(CallData.ContactNumber)
        };

        public CallType GetCallType(string input)
        {
            return CallType.Unknown;
        }

        public DateTime GetDateTime(string input)
        {
            return new DateTime(
                     int.Parse($"20{input[0]}{input[1]}"),
                     int.Parse($"{input[2]}{input[3]}"),
                     int.Parse($"{input[4]}{input[5]}"),
                     int.Parse($"{input[6]}{input[7]}"),
                     int.Parse($"{input[8]}{input[9]}"),
                     int.Parse($"{input[10]}{input[11]}")
                );
        }

        public TimeSpan GetTimeSpan(string input)
        {
            return MediaInfoConvention.GetTimeSpan(input);
        }
    }
}