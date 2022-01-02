namespace Finance.Domain.Yahoo;

public class YahooConstants
{
    public static readonly string BasePath = "https://yh-finance.p.rapidapi.com/";
    public static readonly string RapidApiHeaderKey = "x-rapidapi-key";
    public static readonly string HostHeaderKey = "x-rapidapi-host";
    public static readonly string HostHeaderValue = "yh-finance.p.rapidapi.com";    

    public static class Endpoints
    {
        public static readonly string GetChart = "/stock/v3/get-chart";
        public static readonly string GetHistoryData = "stock/v3/get-historical-data";
    }
}