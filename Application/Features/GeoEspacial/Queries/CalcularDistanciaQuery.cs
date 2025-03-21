using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.GeoEspacial.Queries;

/// <summary>
///     Calcula a distância entre dois CEPs ou coordenadas
/// </summary>
public record CalcularDistanciaQuery : IRequest<RespostaDistanciaDTO>
{
    // Pelo menos um par de identificação deve ser fornecido (CEP ou coordenadas)
    public string? CEPOrigem { get; init; }
    public string? CEPDestino { get; init; }
    public double? LatitudeOrigem { get; init; }
    public double? LongitudeOrigem { get; init; }
    public double? LatitudeDestino { get; init; }
    public double? LongitudeDestino { get; init; }
}