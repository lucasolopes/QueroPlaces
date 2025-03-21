using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Bairro.Queries;

/// <summary>
///     Pesquisa bairros por nome (parcial)
/// </summary>
public record PesquisarBairroQuery : IRequest<ResultadoBuscaDTO<BairroDTO>>
{
    public string? Nome { get; init; }
    public string? Localidade { get; init; }
    public string? UF { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 10;
}