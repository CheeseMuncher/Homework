using Paymentsense.Coding.Challenge.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Services
{
    /// <summary>
    /// Wrapper for the nuget client so we can mock it and we'll replace with our own later
    /// </summary>
    public interface IRestCountriesApi
    {
        Task<IEnumerable<Country>> GetAllCountriesAsync();
    }
}