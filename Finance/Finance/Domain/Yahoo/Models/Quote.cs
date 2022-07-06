namespace Finance.Domain.Yahoo.Models;

public class Quote
{
    public decimal?[] open { get; set; } = null!;
    public decimal?[] low { get; set; } = null!;
    public decimal?[] close { get; set; } = null!;
    public decimal?[] volume { get; set; } = null!;
    public decimal?[] high { get; set; } = null!;
} 