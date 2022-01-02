namespace Finance.Domain.Yahoo.Models;

public class DividendSet
{
    public Dictionary<string, DividendPayout> dividends {get;set;} = null!;
}