using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paymentsense.Coding.Challenge.Api.Models;
using Paymentsense.Coding.Challenge.Api.Services;
using System.Threading.Tasks;

namespace Paymentsense.Coding.Challenge.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Country[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var result = await _countryService.GetAllCountriesAsync();
            return Ok(result);
        }

        [HttpGet("/countries/{alpha3Code}/flag")]
        public async Task<IActionResult> Get(string alpha3Code)
        {
            var bytes = await _countryService.GetFlagAsync(alpha3Code.ToLower());
            return File(bytes, "image/svg+xml");
        }
    }
}