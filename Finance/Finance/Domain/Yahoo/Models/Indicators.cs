namespace Finance.Domain.Yahoo.Models;

public class Indicators
{
    public Quote[] quote { get; set; } = null!;
    public AdjClose[] adjclose { get; set; } = null!;
} 