namespace Finance.Domain.Yahoo.Models;

public class Event
{
    public int data { get; set; }
    public string type { get; set; } = null!;
    public long date { get; set; }
    public decimal amount { get; set; }
}