using Newtonsoft.Json.Linq;
using Paymentsense.Coding.Challenge.Api.Models;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Services
{
    public class RestCountriesApi : IRestCountriesApi
    {
        private readonly string _basePath;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly HttpClient _httpClient;

        public RestCountriesApi(HttpClient httpClient, ApiConfig config)
        {
            _basePath = config.RestCountriesBasePath;
            _retryPolicy = Policy.Handle<HttpRequestException>().WaitAndRetryAsync(config.RetryBackoffPeriodsMilliseconds.Select(i => TimeSpan.FromMilliseconds(i)).ToArray());
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync($"{_basePath}rest/v2/all");
                if (response.IsSuccessStatusCode)
                {
                    JArray jsonArray = JArray.Parse(await response.Content.ReadAsStringAsync());
                    return jsonArray.ToObject<List<Country>>();
                }
                // TODO Logging
                throw new HttpRequestException("Something went wrong fetching country data");
            });
        }

        public async Task<byte[]> GetFlagAsync(string alpha3Code)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var response = await _httpClient.GetAsync($"{_basePath}data/{alpha3Code.ToLower()}.svg");
                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsByteArrayAsync().Result;

                // TODO Logging
                throw new HttpRequestException("Something went wrong fetching country data");
            });
        }
    }
}