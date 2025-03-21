using Application.Extensions.Features.Endereco.Queries;
using FluentValidation;

namespace Application.Extensions.Features.Endereco.Validators;

public class ValidarCepQueryValidator : AbstractValidator<ValidarCepQuery>
{
    public ValidarCepQueryValidator()
    {
        RuleFor(x => x.CEP)
            .NotEmpty().WithMessage("CEP é obrigatório")
            .Length(8).WithMessage("CEP deve ter 8 dígitos")
            .Matches("^[0-9]+$").WithMessage("CEP deve conter apenas números");
    }
}