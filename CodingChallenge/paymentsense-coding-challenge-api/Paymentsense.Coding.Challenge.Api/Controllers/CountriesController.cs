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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _countryService.GetAllCountriesAsync();
                return Ok(result);
            }
            catch
            {
                // TODO Logging
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("/countries/{alpha3Code}/flag")]
        [ProducesResponseType(typeof(Country[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string alpha3Code)
        {
            try
            {
                var bytes = await _countryService.GetFlagAsync(alpha3Code);
                if (bytes is null)
                    // TODO Logging
                    return new NotFoundResult();

                return File(bytes, "image/svg+xml");
            }
            catch
            {
                // TODO Logging
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}