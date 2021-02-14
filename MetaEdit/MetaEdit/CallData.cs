using System;

namespace MetaEdit
{
    public class CallData
    {
        public DateTime CallTime { get; set; }
        public TimeSpan CallDuration { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public CallType CallType { get; set; }

        public override string ToString()
        {
            return $"{CallTime.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss")}_{CallDuration.ToString(@"hh\.mm\.ss")}_{CallTypeUk}_{ContactNumber ?? "Unknown"}_{ContactName ?? "Unknown"}.amr";
        }

        private string CallTypeUk => CallType == CallType.Dialed ? "Dialled" : $"{CallType}";
    }

    public enum CallType
    {
        Dialed, // US spelling is deliberate
        Received,
        Missed
    }
}