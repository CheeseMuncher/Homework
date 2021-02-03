using System;

namespace MetaEdit
{
    public class CallData
    {
        public DateTime CallTime { get; set; }
        public DateTime CallDuration { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
        public CallType CallType { get; set; }
    }

    public enum CallType
    {
        Dialed, // US spelling is deliberate
        Received,
        Missed
    }
}