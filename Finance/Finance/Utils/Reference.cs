using System;
using System.Globalization;

namespace Finance.Utils;

public static class Reference
{
    public static DateTime[] GetMarketDays(DateTime start, DateTime end) =>
        GetRangeDates(start, end)
            .Where(date => date.IsMarketDay())
            .ToArray();

    private static IEnumerable<DateTime> GetRangeDates(DateTime start, DateTime end)
    {
        for (DateTime i = start; i <= end; i = i.AddDays(1))
            yield return i;     
    }

    /// <summary>
    /// Indicates if a day is to be excluded from both US and UK exchanges
    /// It's a market day in the US or the UK, then it should be included
    /// </summary>
    /// <remarks>
    /// Public holidays: 
    /// Christmas day or the following Monday, 
    /// New Year's day or the following Monday, 
    /// Good Friday
    /// </remarks>
    /// <returns>False if the supplied date is a weekend or one of the above public holidays</returns>
    private static bool IsMarketDay(this DateTime date) =>
        date.IsWeekday() && !date.IsXmasHoliday() && !date.IsNewYearHoliday() && !date.IsGoodFriday();

}
