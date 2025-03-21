using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Endereco.Queries;

/// <summary>
///     Pesquisa endereços completos por diversos critérios
/// </summary>
public record PesquisarEnderecoQuery : IRequest<ResultadoBuscaDTO<EnderecoDTO>>
{
    public string? CEP { get; init; }
    public string? Logradouro { get; init; }
    public string? Bairro { get; init; }
    public string? Localidade { get; init; }
    public string? UF { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 10;
}