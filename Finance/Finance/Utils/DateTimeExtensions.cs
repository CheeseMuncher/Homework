using System;
using System.Globalization;

namespace Finance.Utils;

public static class DateTimeExtensions
{
    private static DateTime _epoch = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
    public static DateTime UnixToDateTime(this long ticks) => _epoch.AddSeconds(ticks);
}