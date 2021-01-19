using Microsoft.Extensions.Caching.Memory;
using Paymentsense.Coding.Challenge.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Services
{
    public class CachedCountryService : CountryService
    {
        private const string AllCountryDataKey = "Countries";
        private const string AllCountryCodeKey = "Codes";
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CachedCountryService(IRestCountriesApi restCountriesApi, IMemoryCache cache, ApiConfig config) : base(restCountriesApi)
        {
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(config.CacheLifeTimeSeconds));
        }

        public override async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            if (_cache.TryGetValue(AllCountryDataKey, out IEnumerable<Country> data))
                return data;

            var result = await base.GetAllCountriesAsync();
            _cache.Set(AllCountryDataKey, result, _cacheOptions);
            _cache.Set(AllCountryCodeKey, result.Select(c => c.Alpha3Code).ToArray());
            return result;
        }

        public override async Task<byte[]> GetFlagAsync(string alpha3Code)
        {
            if (!_cache.TryGetValue(AllCountryCodeKey, out string[] codes))
            {
                await GetAllCountriesAsync();
                codes = _cache.Get<string[]>(AllCountryCodeKey);
            }
            if (!codes.Contains(alpha3Code))
                return null;

            if (_cache.TryGetValue(alpha3Code, out byte[] data))
                return data;

            var result = await base.GetFlagAsync(alpha3Code);
            _cache.Set(AllCountryDataKey, result, _cacheOptions);
            return result;
        }
    }
}