using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaEdit.Conventions
{
    public class TotalRecallConvention : IDecodeConvention
    {
        public HashSet<string> Extensions { get; } = new HashSet<string> { ".amr", ".mp3" };

        public HashSet<string> Separators { get; } = new HashSet<string> { "_", " (", ")" };

        public string[] Convention { get; } = new[]
        {
            nameof(CallData.CallTime),
            nameof(CallData.CallType),
            nameof(CallData.ContactName),
            nameof(CallData.ContactNumber)
        };

        public CallType GetCallType(string input)
        {
            if (input == "In")
                return CallType.Received;

            if (input == "Out")
                return CallType.Dialed;

            throw new ArgumentException($"{nameof(TotalRecallConvention)}.{nameof(GetCallType)} was unable to convert input {input} into {nameof(CallType)}");
        }

        public DateTime GetDateTime(string input)
        {
            var components = input.Split("@").SelectMany(c => c.Split("-")).Select(c => int.Parse(c)).ToArray();
            return new DateTime(components[0], components[1], components[2], components[3], components[4], components[5]);
        }

        public TimeSpan GetTimeSpan(string input)
        {
            return MediaInfoConvention.GetTimeSpan(input);
        }
    }
}