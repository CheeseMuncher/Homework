namespace Finance.Domain.TraderMade.Models;

public class ForexHistoryResponse
{
    public DateTime date { get; set; }
    public DateTime request_time { get; set; }
    public string endpoint { get; set; }
    public Quote[] quotes { get; set; } = null!;
}