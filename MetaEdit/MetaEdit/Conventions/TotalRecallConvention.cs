using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaEdit.Conventions
{
    public class TotalRecallConvention : IDecodeConvention
    {
        private const string _milliseconds = "ms";
        private const string _seconds = "s";
        private const string _minutes = "min";
        private const string _hours = "h";
        private readonly HashSet<string> _extensions = new HashSet<string> { ".amr" };
        private readonly HashSet<string> _separators = new HashSet<string> { "_", " (", ")" };

        private readonly string[] _convention = new[]
        {
            nameof(CallData.CallTime),
            nameof(CallData.CallType),
            nameof(CallData.ContactName),
            nameof(CallData.ContactNumber)
        };

        public HashSet<string> Extensions => _extensions;

        public HashSet<string> Separators => _separators;

        public string[] Convention => _convention;

        public CallType GetCallType(string input)
        {
            if (input == "In")
                return CallType.Received;

            if (input == "Out")
                return CallType.Dialed;

            throw new ArgumentException();
        }

        public DateTime GetDateTime(string input)
        {
            var components = input.Split("@").SelectMany(c => c.Split("-")).Select(c => int.Parse(c)).ToArray();
            return new DateTime(components[0], components[1], components[2], components[3], components[4], components[5]);
        }

        public TimeSpan GetTimeSpan(string input)
        {
            if (input.EndsWith(_milliseconds))
            {
                var split = input.Substring(0, input.Length - 3).Split(_seconds, StringSplitOptions.RemoveEmptyEntries);
                if (split.Count() == 1)
                    return new TimeSpan(0, 0, 0, 0, int.Parse(split.Single()));

                return new TimeSpan(0, 0, 0, int.Parse(split.First()), int.Parse(split.Last()));
            }

            if (input.EndsWith(_seconds))
            {
                var split = input.Substring(0, input.Length - 2).Split(_minutes, StringSplitOptions.RemoveEmptyEntries);
                return new TimeSpan(0, 0, int.Parse(split.First()), int.Parse(split.Last()), 0);
            }

            if (input.EndsWith(_minutes))
            {
                var split = input.Substring(0, input.Length - 4).Split(_hours, StringSplitOptions.RemoveEmptyEntries);
                return new TimeSpan(0, int.Parse(split.First()), int.Parse(split.Last()), 0, 0);
            }

            return new TimeSpan(0, 0, 0, 0, 0);
        }
    }
}