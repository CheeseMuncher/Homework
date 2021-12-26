using Finance.Domain.Dividends;

namespace Finance.Domain.Yahoo;

public class Result
{
    public Dictionary<string, object> meta {get;set;}
    public long[] timestamp {get;set;}
    public Indicators indicators {get;set;}
    public DividendSet events {get;set;}
}