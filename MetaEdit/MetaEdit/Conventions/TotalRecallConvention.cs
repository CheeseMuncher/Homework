using System;
using System.Collections.Generic;
using System.Linq;

namespace MetaEdit.Conventions
{
    public class TotalRecallConvention : IDecodeConvention
    {
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
    }
}