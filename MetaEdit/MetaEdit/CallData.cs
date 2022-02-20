using System;

namespace MetaEdit
{
    public class CallData
    {
        public DateTime CallTime { get; set; }
        public TimeSpan CallDuration { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public string FileExtension { get; set; }
        public CallType CallType { get; set; }

        public override string ToString()
        {
            return $"{CallTime.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss")}_{CallDuration.ToString(@"hh\.mm\.ss")}_{CallTypeUk}_{ContactNumber ?? "Unknown"}_{ContactName ?? "Unknown"}.{FileExtension}";
        }

        private string CallTypeUk => CallType == CallType.Dialed ? "Dialled" : $"{CallType}";
    }

    public enum CallType
    {
        Unknown = 0,
        Dialed = 1, // US spelling is deliberate
        Received = 2,
        Missed = 3,
        Rejected = 4
    }
}