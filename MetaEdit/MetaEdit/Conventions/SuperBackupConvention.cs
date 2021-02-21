using System;
using System.Collections.Generic;
using System.Globalization;

namespace MetaEdit.Conventions
{
    public class SuperBackupConvention : IDecodeConvention
    {
        public HashSet<string> Extensions { get; } = new HashSet<string> { ".csv" };

        public HashSet<string> Separators { get; } = new HashSet<string> { "," };

        public string[] Convention { get; } = new[]
        {
            nameof(CallData.ContactName),
            nameof(CallData.ContactNumber),
            nameof(CallData.CallTime),
            nameof(CallData.CallType),
            nameof(CallData.CallDuration)
        };

        public CallType GetCallType(string input)
        {
            if (Enum.TryParse(input, out CallType callType))
                return callType;

            throw new ArgumentException($"{nameof(SuperBackupConvention)}.{nameof(GetCallType)} was unable to convert {input} to {nameof(CallType)}");
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