namespace Finance.Domain.Yahoo;

public class Result
{
    public Dictionary<string, object> meta {get;set;} = null!;
    public long[] timestamp {get;set;} = null!;
    public Indicators indicators {get;set;} = null!;
    public DividendSet events {get;set;} = null!;
}