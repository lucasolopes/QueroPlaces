namespace Application.Extensions.Common.Models;

public record RespostaDistanciaDTO
{
    public double DistanciaEmKm { get; init; }
    public string? EnderecoOrigem { get; init; }
    public string? EnderecoDestino { get; init; }
    public double? LatitudeOrigem { get; init; }
    public double? LongitudeOrigem { get; init; }
    public double? LatitudeDestino { get; init; }
    public double? LongitudeDestino { get; init; }
}