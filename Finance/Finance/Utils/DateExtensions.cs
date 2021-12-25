using System;
using System.Globalization;

namespace Finance.Utils;

public static class DateExtensions
{
    public static bool IsWeekday(this DateTime day) => day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday;
    public static bool IsXmas(this DateTime day) => day.Month == 12 && day.Day == 25;
    public static bool IsFirstWorkday(this DateTime day) => day.Month == 1
        && ((day.Day == 1 && day.IsWeekday())
             || (day.Day <= 3 && day.DayOfWeek == DayOfWeek.Monday));
}
