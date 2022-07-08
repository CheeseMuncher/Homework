using System;
using System.Globalization;

namespace Finance.Utils;

public static class DateExtensions
{
    private static HashSet<DateTime> _goodFridays = new HashSet<DateTime>
    {
        new DateTime(2012,04,06),
        new DateTime(2013,03,29),
        new DateTime(2014,04,18),
        new DateTime(2015,04,03),
        new DateTime(2016,03,25),
        new DateTime(2017,04,14),
        new DateTime(2018,03,30),
        new DateTime(2019,04,19),
        new DateTime(2020,04,10),
        new DateTime(2021,04,02),
        new DateTime(2022,04,15),
        new DateTime(2023,04,07),
        new DateTime(2024,03,29),
        new DateTime(2025,04,18),
        new DateTime(2026,04,03)
    };  

    public static bool IsWeekday(this DateTime date) => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

    public static bool IsXmasHoliday(this DateTime date) => date.Month == 12
        && ((date.Day == 25 && date.IsWeekday())
            || (date.DayOfWeek == DayOfWeek.Monday && (date.Day == 26 || date.Day == 27)));

    public static bool IsNewYearHoliday(this DateTime date) => date.Month == 1
        && ((date.Day == 1 && date.IsWeekday())
            || (date.Day <= 3 && date.DayOfWeek == DayOfWeek.Monday));

    public static bool IsGoodFriday(this DateTime date)
    {
        // Alternative to throwing is to fetch data from https://www.gov.uk/bank-holidays.json
        // After a few years we'd end up fetching data with every call.
        // This choice means updating this file every couple of years.
        if (_goodFridays.All(d => d.Year != date.Year))
            throw new Exception($"No Good Friday date stored for year {date.Year}");

        var match = _goodFridays.First(d => d.Year == date.Year);
        return match.Month == date.Month && match.Day == date.Day;
    }
}
