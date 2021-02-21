using System;
using System.Linq;

namespace MetaEdit.Conventions
{
    /// <summary>
    /// Helper class to help decode values from the MediaInfo library
    /// </summary>
    public static class MediaInfoConvention
    {
        private const string _milliseconds = "ms";
        private const string _seconds = "s";
        private const string _minutes = "min";
        private const string _hours = "h";

        public static TimeSpan GetTimeSpan(string input)
        {
            if (input is null)
                return new TimeSpan();

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

            return new TimeSpan();
        }
    }
}