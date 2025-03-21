using Application.Extensions.Features.GeoEspacial.Queries;
using FluentValidation;

namespace Application.Extensions.Features.GeoEspacial.Validators;

public class BuscarEnderecosPorRaioQueryValidator : AbstractValidator<BuscarEnderecosPorRaioQuery>
{
    public BuscarEnderecosPorRaioQueryValidator()
    {
        // Validar que pelo menos uma forma de origem foi fornecida (CEP ou coordenadas)
        RuleFor(x => new { x.CEP, x.Latitude, x.Longitude })
            .Must(x => !string.IsNullOrWhiteSpace(x.CEP) || (x.Latitude.HasValue && x.Longitude.HasValue))
            .WithMessage("É necessário informar o CEP ou coordenadas (latitude/longitude) de origem");

        // Se CEP for fornecido, deve ser válido
        When(x => !string.IsNullOrWhiteSpace(x.CEP), () =>
        {
            RuleFor(x => x.CEP)
                .Length(8).WithMessage("CEP deve ter 8 dígitos")
                .Matches("^[0-9]+$").WithMessage("CEP deve conter apenas números");
        });

        // Se latitude for fornecida, deve ser válida
        When(x => x.Latitude.HasValue, () =>
        {
            RuleFor(x => x.Latitude!.Value)
                .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90");
        });

        // Se longitude for fornecida, deve ser válida
        When(x => x.Longitude.HasValue, () =>
        {
            RuleFor(x => x.Longitude!.Value)
                .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180");
        });

        // Validar raio de busca
        RuleFor(x => x.RaioKm)
            .GreaterThan(0).WithMessage("Raio deve ser maior que 0")
            .LessThanOrEqualTo(100).WithMessage("Raio não pode ser maior que 100 km");

        // Se tipo de endereço for fornecido, deve ser válido
        When(x => !string.IsNullOrWhiteSpace(x.TipoEndereco), () =>
        {
            RuleFor(x => x.TipoEndereco)
                .Must(tipo => new[] { "localidade", "logradouro", "bairro", "todos" }.Contains(tipo.ToLower()))
                .WithMessage("Tipo de endereço deve ser 'localidade', 'logradouro', 'bairro' ou 'todos'");
        });

        // Paginação
        RuleFor(x => x.Pagina)
            .GreaterThan(0).WithMessage("Página deve ser maior que zero");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("Tamanho da página deve estar entre 1 e 100");
    }
}