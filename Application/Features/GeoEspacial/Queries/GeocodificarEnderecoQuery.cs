using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.GeoEspacial.Queries;

/// <summary>
///     Geocodifica um endereço (converte endereço para coordenadas)
/// </summary>
public record GeocodificarEnderecoQuery : IRequest<EnderecoDTO>
{
    public string? CEP { get; init; }
    public string? Logradouro { get; init; }
    public string? Numero { get; init; }
    public string? Complemento { get; init; }
    public string? Bairro { get; init; }
    public string? Localidade { get; init; }
    public string? UF { get; init; }
}