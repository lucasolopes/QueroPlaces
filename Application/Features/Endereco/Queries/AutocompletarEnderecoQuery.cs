using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Endereco.Queries;

/// <summary>
///     Autocompletar de endereço
/// </summary>
public record AutocompletarEnderecoQuery : IRequest<IEnumerable<EnderecoDTO>>
{
    public string Termo { get; init; } = null!;
    public string? Tipo { get; init; } // logradouro, bairro, localidade
    public string? UF { get; init; }
    public int Limite { get; init; } = 10;
}