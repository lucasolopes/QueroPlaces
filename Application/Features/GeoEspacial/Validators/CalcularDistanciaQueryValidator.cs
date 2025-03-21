using Application.Extensions.Features.GeoEspacial.Queries;
using FluentValidation;

namespace Application.Extensions.Features.GeoEspacial.Validators;

public class CalcularDistanciaQueryValidator : AbstractValidator<CalcularDistanciaQuery>
{
    public CalcularDistanciaQueryValidator()
    {
        // Validar que pelo menos uma forma de origem foi fornecida (CEP ou coordenadas)
        RuleFor(x => new { x.CEPOrigem, x.LatitudeOrigem, x.LongitudeOrigem })
            .Must(x => !string.IsNullOrWhiteSpace(x.CEPOrigem) ||
                       (x.LatitudeOrigem.HasValue && x.LongitudeOrigem.HasValue))
            .WithMessage("É necessário informar o CEP de origem ou coordenadas (latitude/longitude) de origem");

        // Validar que pelo menos uma forma de destino foi fornecida (CEP ou coordenadas)
        RuleFor(x => new { x.CEPDestino, x.LatitudeDestino, x.LongitudeDestino })
            .Must(x => !string.IsNullOrWhiteSpace(x.CEPDestino) ||
                       (x.LatitudeDestino.HasValue && x.LongitudeDestino.HasValue))
            .WithMessage("É necessário informar o CEP de destino ou coordenadas (latitude/longitude) de destino");

        // Se CEP de origem for fornecido, deve ser válido
        When(x => !string.IsNullOrWhiteSpace(x.CEPOrigem), () =>
        {
            RuleFor(x => x.CEPOrigem)
                .Length(8).WithMessage("CEP de origem deve ter 8 dígitos")
                .Matches("^[0-9]+$").WithMessage("CEP de origem deve conter apenas números");
        });

        // Se CEP de destino for fornecido, deve ser válido
        When(x => !string.IsNullOrWhiteSpace(x.CEPDestino), () =>
        {
            RuleFor(x => x.CEPDestino)
                .Length(8).WithMessage("CEP de destino deve ter 8 dígitos")
                .Matches("^[0-9]+$").WithMessage("CEP de destino deve conter apenas números");
        });

        // Se latitude de origem for fornecida, deve ser válida
        When(x => x.LatitudeOrigem.HasValue, () =>
        {
            RuleFor(x => x.LatitudeOrigem!.Value)
                .InclusiveBetween(-90, 90).WithMessage("Latitude de origem deve estar entre -90 e 90");
        });

        // Se longitude de origem for fornecida, deve ser válida
        When(x => x.LongitudeOrigem.HasValue, () =>
        {
            RuleFor(x => x.LongitudeOrigem!.Value)
                .InclusiveBetween(-180, 180).WithMessage("Longitude de origem deve estar entre -180 e 180");
        });

        // Se latitude de destino for fornecida, deve ser válida
        When(x => x.LatitudeDestino.HasValue, () =>
        {
            RuleFor(x => x.LatitudeDestino!.Value)
                .InclusiveBetween(-90, 90).WithMessage("Latitude de destino deve estar entre -90 e 90");
        });

        // Se longitude de destino for fornecida, deve ser válida
        When(x => x.LongitudeDestino.HasValue, () =>
        {
            RuleFor(x => x.LongitudeDestino!.Value)
                .InclusiveBetween(-180, 180).WithMessage("Longitude de destino deve estar entre -180 e 180");
        });
    }
}