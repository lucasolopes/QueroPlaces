using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Localidade.Queries;

/// <summary>
///     Pesquisa localidades por nome (parcial)
/// </summary>
public record PesquisarLocalidadeQuery : IRequest<ResultadoBuscaDTO<LocalidadeDTO>>
{
    public string? Nome { get; init; }
    public string? UF { get; init; }
    public int? CodigoIBGE { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 10;
}