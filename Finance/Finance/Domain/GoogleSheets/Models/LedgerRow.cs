namespace Finance.Domain.GoogleSheets.Models;

public class LedgerInputRow
{
    public static string[] HeaderRow => new [] { "Date", "Currency", "Portfolio", "Code", "Consideration", "Units" };

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
    public LedgerRow(DateOnly date, string currency, string portfolio, string code, decimal consideration, int units)
        : base(date, currency, portfolio, code, consideration, units)
    {
        
    }
    public LedgerRowType RowType { get; }
    public decimal Price { get; }
    public decimal LocalExposure { get; }
    public decimal PositionExposure { get; }
    public decimal PortfolioExposure { get; }
}

public enum LedgerRowType
{
    Cash,
    Stock,
    Fund    
}