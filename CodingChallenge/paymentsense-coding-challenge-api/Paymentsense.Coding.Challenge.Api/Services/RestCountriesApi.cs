using Paymentsense.Coding.Challenge.Api.Models;
using RESTCountries.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Services
{
    public class RestCountriesApi : IRestCountriesApi
    {
        // TODO replace client with our own code
        public async Task<IEnumerable<Country>> GetAllCountriesAsync() =>
            (await RESTCountriesAPI.GetAllCountriesAsync())
                .Select(country => new Country
                {
                    Alpha3Code = country.Alpha3Code,
                    Name = country.Name,
                    Flag = country.Flag
                });
    }
}