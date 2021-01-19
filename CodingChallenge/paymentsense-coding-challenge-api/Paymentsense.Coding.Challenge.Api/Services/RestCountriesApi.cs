using Paymentsense.Coding.Challenge.Api.Models;
using RESTCountries.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Services
{
    public class RestCountriesApi : IRestCountriesApi
    {
        private readonly HttpClient _httpClient;

        public RestCountriesApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // TODO replace client with our own code
        public async Task<IEnumerable<Country>> GetAllCountriesAsync() =>
            (await RESTCountriesAPI.GetAllCountriesAsync())
                .Select(country => new Country
                {
                    Alpha3Code = country.Alpha3Code,
                    Name = country.Name,
                    Flag = country.Flag,
                    Population = country.Population,
                    Capital = country.Capital,
                    Timezones = country.Timezones,
                    Currencies = country.Currencies.Select(c => new Currency { Name = c.Name, Code = c.Code, Symbol = c.Symbol }),
                    Languages = country.Languages.Select(l => new Language { Name = l.Name }),
                    Borders = country.Borders
                });

        public async Task<byte[]> GetFlagAsync(string alpha3Code)
        {
            var response = await _httpClient.GetAsync($"https://restcountries.eu/data/{alpha3Code.ToLower()}.svg");
            return response.Content.ReadAsByteArrayAsync().Result;
        }
    }
}