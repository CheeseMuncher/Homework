using Paymentsense.Coding.Challenge.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Services
{
    public class CountryService : ICountryService
    {
        private readonly IRestCountriesApi _restCountriesApi;

        public CountryService(IRestCountriesApi restCountriesApi)
        {
            _restCountriesApi = restCountriesApi;
        }

        public virtual async Task<IEnumerable<Country>> GetAllCountriesAsync() => await _restCountriesApi.GetAllCountriesAsync();
    }
}