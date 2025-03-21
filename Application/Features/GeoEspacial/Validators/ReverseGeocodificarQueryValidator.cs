using Application.Extensions.Features.GeoEspacial.Queries;
using FluentValidation;

namespace Application.Extensions.Features.GeoEspacial.Validators;

public class ReverseGeocodificarQueryValidator : AbstractValidator<ReverseGeocodificarQuery>
{
    public ReverseGeocodificarQueryValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180");
    }
}