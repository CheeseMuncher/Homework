namespace Finance.Domain.Models;

public class LedgerRow : LedgerInputRow
{
    public static string[] FullHeaderRow => HeaderRow.Concat(new [] { nameof(Price), nameof(LocalExposure), nameof(PositionExposure), nameof(PortfolioExposure) }).ToArray();

    public LedgerRowType RowType { get; }
    public decimal Price { get; set; }
    public decimal LocalExposure { get; set; }
    public decimal PositionExposure { get; set; }
    public decimal PortfolioExposure { get; set; }
    public IList<object> ToSpreadsheetRow => new List<object>
        {
            Date.ToString("yyyy-MM-dd"),
            ToCell(Currency),
            ToCell(Portfolio),
            ToCell(Code),
            ToCell(Consideration, 2),
            ToCell(Units, 0),
            ToCell(Price, 3),
            ToCell(LocalExposure, 2),
            ToCell(PositionExposure, 2),
            ToCell(PortfolioExposure, 2)
        };

    private static string ToCell(string prop) 
        => string.IsNullOrWhiteSpace(prop) ? "" : prop;
    private static string ToCell(decimal prop, int decimals)
        => prop == 0m ? "" : decimals == 0 ? prop.ToString("0") 
        : prop.ToString($"0.{new string(Enumerable.Repeat('0', decimals).ToArray())}");
}

public enum LedgerRowType
{
    Cash,
    Stock,
    Fund    
}
