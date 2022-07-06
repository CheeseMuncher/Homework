namespace Finance.Domain.Yahoo.Models;

public class HistoryResponse
{
    public Price[] prices { get; set; } = null!;
    public bool isPending { get; set; }
    public long firstTradeDate { get; set; }

    // timeZone / gmtOffset
    // id

    public Event[] eventsData { get; set; } = null!;
}