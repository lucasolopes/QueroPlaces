using Application.Extensions.Features.Endereco.Queries;
using FluentValidation;

namespace Application.Extensions.Features.Endereco.Validators;

public class AutocompletarEnderecoQueryValidator : AbstractValidator<AutocompletarEnderecoQuery>
{
    public AutocompletarEnderecoQueryValidator()
    {
        RuleFor(x => x.Termo)
            .NotEmpty().WithMessage("Termo de busca é obrigatório")
            .MinimumLength(3).WithMessage("Termo de busca deve ter pelo menos 3 caracteres");

        When(x => !string.IsNullOrWhiteSpace(x.Tipo), () =>
        {
            RuleFor(x => x.Tipo)
                .Must(tipo => new[] { "logradouro", "bairro", "localidade" }.Contains(tipo.ToLower()))
                .WithMessage("Tipo deve ser 'logradouro', 'bairro' ou 'localidade'");
        });

        When(x => !string.IsNullOrWhiteSpace(x.UF), () =>
        {
            RuleFor(x => x.UF)
                .Length(2).WithMessage("UF deve ter 2 caracteres");
        });

        RuleFor(x => x.Limite)
            .InclusiveBetween(1, 50).WithMessage("Limite deve estar entre 1 e 50");
    }
}