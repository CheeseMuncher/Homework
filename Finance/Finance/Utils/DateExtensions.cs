using System;
using System.Globalization;

namespace Finance.Utils;

public static class DateExtensions
{
    public static bool IsWeekday(this DateTime day) => day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday;
    public static bool IsXmasHoliday(this DateTime day) => day.Month == 12
        && ((day.Day == 25 && day.IsWeekday())
            || (day.DayOfWeek == DayOfWeek.Monday && (day.Day == 26 || day.Day == 27)));
    public static bool IsNewYearHoliday(this DateTime day) => day.Month == 1
        && ((day.Day == 1 && day.IsWeekday())
            || (day.Day <= 3 && day.DayOfWeek == DayOfWeek.Monday));
}
