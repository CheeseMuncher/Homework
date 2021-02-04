using System;
using System.Linq;

namespace MetaEdit
{
    public class TotalRecallConvention : IDecodeConvention
    {
        private readonly string[] _separators = new[] { "_", " (", ")" };

        private readonly string[] _convention = new[]
        {
            nameof(CallData.CallTime),
            nameof(CallData.CallType),
            nameof(CallData.ContactName),
            nameof(CallData.ContactNumber)
        };

        public string[] Separators => _separators;

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