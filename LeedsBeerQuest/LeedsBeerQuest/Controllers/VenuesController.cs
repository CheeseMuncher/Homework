using System.Text.Json;
using FluentValidation;
using LeedsBeerQuest.Data;
using LeedsBeerQuest.Models;
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Venue>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public ActionResult Get([FromQuery] VenueQuery venueQuery, [FromQuery] string sortKey = "")
    {
        var errors = new HashSet<string>();
        SortKeyType sortKeyType = SortKeyType.Beer;
        if (!string.IsNullOrEmpty(sortKey))
            if (!Enum.TryParse<SortKeyType>(sortKey, true, out sortKeyType))
                errors.Add($"Invalid sort key: {sortKey}, valid sort keys are {string.Join(',', Enum.GetNames<SortKeyType>())}");

        var result = _venueQueryValidator.Validate(venueQuery);
        if (!result.IsValid)
            errors.Add($"Invalid query: {string.Join(',', result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))}");

        if (errors.Any())
            return BadRequest(string.Join(';', errors));

        var venues = _venueRepository.GetVenues(venueQuery, sortKeyType);
        return Ok(JsonSerializer.Serialize(venues));
    }    
}