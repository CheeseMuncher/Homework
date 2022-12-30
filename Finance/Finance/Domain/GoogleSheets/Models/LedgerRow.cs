namespace Finance.Domain.GoogleSheets.Models;

public class LedgerInputRow
{
    public static string[] HeaderRow => new [] { nameof(Date), nameof(Currency), nameof(Portfolio), nameof(Code), nameof(Consideration), nameof(Units) };

    public LedgerInputRow(DateOnly date, string currency, string portfolio, string code, decimal consideration, int units)
    {
        Date = date;
        Currency = currency;
        Portfolio = portfolio;
        Code = code;
        Consideration = consideration;
        Units = units;
    }

    public DateOnly Date { get; }
    public string Currency { get; } = "";
    public string Portfolio { get; } = "";
    public string Code { get; } = "";
    public decimal Consideration { get; }
    public int Units { get; }
}

public class LedgerRow : LedgerInputRow
{
    public static string[] FullHeaderRow => HeaderRow.Concat(new [] { nameof(Price), nameof(LocalExposure), nameof(PositionExposure), nameof(PortfolioExposure) }).ToArray();

    public LedgerRow(DateOnly date, string currency, string portfolio, string code, decimal consideration, int units)
        : base(date, currency, portfolio, code, consideration, units) { }
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