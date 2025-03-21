using Application.Extensions.Features.GeoEspacial.Queries;
using FluentValidation;

namespace Application.Extensions.Features.GeoEspacial.Validators;

public class GeocodificarEnderecoQueryValidator : AbstractValidator<GeocodificarEnderecoQuery>
{
    public GeocodificarEnderecoQueryValidator()
    {
        // Pelo menos um critério de endereço deve ser fornecido
        RuleFor(x => new { x.CEP, x.Logradouro, x.Localidade, x.UF })
            .Must(x => !string.IsNullOrWhiteSpace(x.CEP) ||
                       (!string.IsNullOrWhiteSpace(x.Logradouro) &&
                        !string.IsNullOrWhiteSpace(x.Localidade) &&
                        !string.IsNullOrWhiteSpace(x.UF)))
            .WithMessage("É necessário informar o CEP ou (Logradouro, Localidade e UF)");

        // Se CEP for fornecido, deve ser válido
        When(x => !string.IsNullOrWhiteSpace(x.CEP), () =>
        {
            RuleFor(x => x.CEP)
                .Length(8).WithMessage("CEP deve ter 8 dígitos")
                .Matches("^[0-9]+$").WithMessage("CEP deve conter apenas números");
        });

        // Se UF for fornecida, deve ser válida
        When(x => !string.IsNullOrWhiteSpace(x.UF), () =>
        {
            RuleFor(x => x.UF)
                .Length(2).WithMessage("UF deve ter 2 caracteres");
        });
    }
}