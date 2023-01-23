using FluentValidation;

namespace LeedsBeerQuest;

public class VenueQueryValidator : AbstractValidator<VenueQuery>
{
    public VenueQueryValidator(IVenueRepository venueRepository)
    {
        RuleForEach(query => query.Tags)
            .Must(tag => venueRepository.GetAllTags().Contains(tag))
            .WithMessage((query, tag) => $"Tag value {tag} is invalid");
            
        RuleFor(query => query.Latitude)
            .Must(lat => -90m <= lat && lat <= 90m)
            .WithMessage((query, lat) => $"Latitude value {lat} is invalid");
            
        RuleFor(query => query.Longitude)
            .Must(lng => -180m <= lng && lng <= 180m)
            .WithMessage((query, lng) => $"Longitude value {lng} is invalid");
            
        RuleFor(query => query.MinimumBeerStars)
            .Must(stars => IsValidStarMinimum(stars))
            .WithMessage((query, stars) => $"MinimumBeerStars value {stars} is invalid");

        RuleFor(query => query.MinimumAtmosphereStars)
            .Must(stars => IsValidStarMinimum(stars))
            .WithMessage((query, stars) => $"MinimumAtmosphereStars value {stars} is invalid");

        RuleFor(query => query.MinimumAmenitiesStars)
            .Must(stars => IsValidStarMinimum(stars))
            .WithMessage((query, stars) => $"MinimumAmenitiesStars value {stars} is invalid");

        RuleFor(query => query.MinimumValueStars)
            .Must(stars => IsValidStarMinimum(stars))
            .WithMessage((query, stars) => $"MinimumValueStars value {stars} is invalid");

    }

    private bool IsValidStarMinimum(decimal? value) => 0 <= (value ?? 0) && (value ?? 0) <= 5m;    
}
