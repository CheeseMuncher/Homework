namespace Finance.Utils;

public static class StringExtensions
{
    public static string HandleIndex(this string stock) =>
        stock.EndsWith(".L") ? stock[..^2] : stock;
}
