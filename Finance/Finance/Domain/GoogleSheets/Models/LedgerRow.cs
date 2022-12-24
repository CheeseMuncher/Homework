namespace Finance.Domain.GoogleSheets.Models;

public class LedgerInputRow
{
    public static string[] HeaderRow => new [] { "Date", "Currency", "Portfolio", "Code", "Consideration", "Units" };

    public DateOnly Date { get; set;}
    public string Currency { get; } = "";
    public string Portfolio { get; } = "";
    public string Code { get; } = "";
    public decimal Consideration { get; }
    public int Units { get; }

}