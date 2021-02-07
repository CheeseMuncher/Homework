using System;
using System.Collections.Generic;
using System.Globalization;

namespace MetaEdit.Conventions
{
    public class SuperBackupConvention : IDecodeConvention
    {
        private readonly HashSet<string> _extensions = new HashSet<string> { ".csv" };
        private readonly HashSet<string> _separators = new HashSet<string> { "," };

        private readonly string[] _convention = new[]
{
            nameof(CallData.ContactName),
            nameof(CallData.ContactNumber),
            nameof(CallData.CallTime),
            nameof(CallData.CallType),
            nameof(CallData.CallDuration)
        };

        public HashSet<string> Extensions => _extensions;

        public HashSet<string> Separators => _separators;

        public string[] Convention => _convention;

        public CallType GetCallType(string input)
        {
            if (Enum.TryParse(input, out CallType callType))
                return callType;

            throw new ArgumentException();
        }

        public DateTime GetDateTime(string input)
        {
            DateTime.TryParseExact(input, "dd MMM yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime converted);
            return converted;
        }

        public TimeSpan GetTimeSpan(string input)
        {
            return TimeSpan.Parse(input);
        }
    }
}