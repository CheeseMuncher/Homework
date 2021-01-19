namespace Paymentsense.Coding.Challenge.Api
{
    public class ApiConfig
    {
        public int HandlerLifetimeSeconds { get; set; }
        public int CacheLifeTimeSeconds { get; set; }
        public string RestCountriesBasePath { get; set; }
        public int[] RetryBackoffPeriodsMilliseconds { get; set; }
    }
}