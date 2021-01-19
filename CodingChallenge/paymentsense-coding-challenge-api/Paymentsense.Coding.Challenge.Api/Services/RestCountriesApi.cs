using Newtonsoft.Json.Linq;
using Paymentsense.Coding.Challenge.Api.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Services
{
    public class RestCountriesApi : IRestCountriesApi
    {
        private const string BasePath = "https://restcountries.eu/";
        private readonly HttpClient _httpClient;

        public RestCountriesApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            var response = await _httpClient.GetAsync($"{BasePath}rest/v2/all");
            JArray jsonArray = JArray.Parse(await response.Content.ReadAsStringAsync());
            return jsonArray.ToObject<List<Country>>();
        }

        public async Task<byte[]> GetFlagAsync(string alpha3Code)
        {
            var response = await _httpClient.GetAsync($"{BasePath}data/{alpha3Code.ToLower()}.svg");
            return response.Content.ReadAsByteArrayAsync().Result;
        }
    }
}