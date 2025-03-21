namespace Application.Extensions.Common.Models;

public record CalculoDistanciaDTO
{
    public string? CEPOrigem { get; init; }
    public string? CEPDestino { get; init; }
    public double? LatitudeOrigem { get; init; }
    public double? LongitudeOrigem { get; init; }
    public double? LatitudeDestino { get; init; }
    public double? LongitudeDestino { get; init; }
}