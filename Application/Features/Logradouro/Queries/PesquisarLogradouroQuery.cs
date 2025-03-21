using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Logradouro.Queries;

/// <summary>
///     Pesquisa logradouros complexa
/// </summary>
public record PesquisarLogradouroQuery : IRequest<ResultadoBuscaDTO<LogradouroDTO>>
{
    public string? Nome { get; init; }
    public string? TipoLogradouro { get; init; }
    public string? Bairro { get; init; }
    public string? Localidade { get; init; }
    public string? UF { get; init; }
    public string? CEP { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 10;
}