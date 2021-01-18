using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTCountries.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountriesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var result = await RESTCountriesAPI.GetAllCountriesAsync();
            var data = result.Select(country => country.Name).ToArray();
            return Ok(data);
        }
    }
}