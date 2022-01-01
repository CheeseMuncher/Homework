namespace Finance.Utils;

public static class StringExtensions
{
    public static string HandleSuffix(this string stock) =>
        
        stock.EndsWith(".L")
            || stock.EndsWith("=X")
                ? stock[..^2] 
                : stock;
}
