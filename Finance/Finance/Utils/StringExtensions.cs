namespace Finance.Utils;

public static class StringExtensions
{
    public static string HandleSuffix(this string stock)
    {
        if (stock.EndsWith("=X"))
            return $"USD{stock[..^2]}";

        if (stock.EndsWith(".L"))
            return stock[..^2];

        return stock;
    }
}
