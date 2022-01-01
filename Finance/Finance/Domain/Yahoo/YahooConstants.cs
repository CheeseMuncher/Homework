namespace Finance.Domain.Yahoo;

public class YahooConstants
{
    public static readonly string BasePath = "https://yh-finance.p.rapidapi.com/";
   
    public static readonly string HeaderKeyKey = "x-rapidapi-key";
    public static readonly string HeaderHostKey = "x-rapidapi-host";
    public static readonly string HeaderHostValue = "yh-finance.p.rapidapi.com";    

    public static class Endpoints
    {
        public static readonly string GetChart = "/stock/v3/get-chart";
        public static readonly string GetHistoricalData = "stock/v3/get-historical-data";
    }
}