using Application.Extensions.Features.Endereco.Queries;
using FluentValidation;

namespace Application.Extensions.Features.Endereco.Validators;

public class PesquisarEnderecoQueryValidator : AbstractValidator<PesquisarEnderecoQuery>
{
    public PesquisarEnderecoQueryValidator()
    {
        // Pelo menos um critério de pesquisa deve ser fornecido
        RuleFor(x => new { x.CEP, x.Logradouro, x.Bairro, x.Localidade, x.UF })
            .Must(x => !string.IsNullOrWhiteSpace(x.CEP) ||
                       !string.IsNullOrWhiteSpace(x.Logradouro) ||
                       !string.IsNullOrWhiteSpace(x.Bairro) ||
                       !string.IsNullOrWhiteSpace(x.Localidade) ||
                       !string.IsNullOrWhiteSpace(x.UF))
            .WithMessage("Pelo menos um critério de pesquisa deve ser fornecido");

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

        // Paginação
        RuleFor(x => x.Pagina)
            .GreaterThan(0).WithMessage("Página deve ser maior que zero");

        RuleFor(x => x.TamanhoPagina)
            .InclusiveBetween(1, 100).WithMessage("Tamanho da página deve estar entre 1 e 100");
    }
}