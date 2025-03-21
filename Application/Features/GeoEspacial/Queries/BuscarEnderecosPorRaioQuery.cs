using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.GeoEspacial.Queries;

/// <summary>
///     Busca endereços próximos a um ponto por raio
/// </summary>
public record BuscarEnderecosPorRaioQuery : IRequest<ResultadoBuscaDTO<EnderecoDTO>>
{
    // Origem pode ser um CEP ou coordenadas
    public string? CEP { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }

    // Parâmetros de busca
    public double RaioKm { get; init; } = 5.0;
    public string? TipoEndereco { get; init; } // localidade, logradouro, etc
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 10;
}