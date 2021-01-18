using Microsoft.Extensions.Caching.Memory;
using Paymentsense.Coding.Challenge.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Services
{
    public class CachedCountryService : CountryService
    {
        private const string _key = "Countries";
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public CachedCountryService(IRestCountriesApi restCountriesApi, IMemoryCache cache) : base(restCountriesApi)
        {
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
        }

        public override async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            if (_cache.TryGetValue(_key, out IEnumerable<Country> data))
                return data;

            var result = await base.GetAllCountriesAsync();
            _cache.Set(_key, result, _cacheOptions);
            return result;
        }
    }
}