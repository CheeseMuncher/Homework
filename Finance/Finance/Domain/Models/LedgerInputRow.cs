namespace Finance.Domain.Models;

public class LedgerInputRow
{
    public static string[] HeaderRow => new [] { nameof(Date), nameof(Currency), nameof(Portfolio), nameof(Code), nameof(Consideration), nameof(Units) };

    public DateOnly Date { get; init; }
    public string Currency { get; init; }
    public string Portfolio { get; init; }
    public string Code { get; init; }
    public decimal Consideration { get; init; }
    public int Units { get; init; }
}
