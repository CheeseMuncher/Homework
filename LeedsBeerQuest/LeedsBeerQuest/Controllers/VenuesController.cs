using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace LeedsBeerQuest.Controllers;

[Route("[Controller]")]
public class VenuesController : ControllerBase
{
    private readonly IVenueRepository _venueRepository;
    private readonly IValidator<VenueQuery> _venueQueryValidator;

    public VenuesController(IVenueRepository venueRepository, IValidator<VenueQuery> venueQueryValidator)
    {
        _venueRepository = venueRepository;
        _venueQueryValidator = venueQueryValidator;
    }

    [HttpGet]
    public ActionResult Get([FromQuery] VenueQuery venueQuery, [FromQuery] string sortValue)
    {
        var result = _venueQueryValidator.Validate(venueQuery);
        if (!result.IsValid)
            return BadRequest($"Invalid query: {string.Join(',', result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))}");

        var venues = _venueRepository.GetVenues(venueQuery);
        return Ok(JsonSerializer.Serialize(venues));
    }
}