namespace Finance.Domain.Yahoo;

public class Quote
{
    public decimal?[] open { get; set; }
    public decimal?[] low { get; set; }
    public decimal?[] close { get; set; }
    public decimal?[] volume { get; set; }
    public decimal?[] high { get; set; }
}