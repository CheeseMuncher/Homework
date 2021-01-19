using Paymentsense.Coding.Challenge.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Services
{
    public interface IRestCountriesApi
    {
        Task<IEnumerable<Country>> GetAllCountriesAsync();

        Task<byte[]> GetFlagAsync(string alpha3Code);
    }
}