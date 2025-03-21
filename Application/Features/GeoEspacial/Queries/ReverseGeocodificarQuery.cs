using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.GeoEspacial.Queries;

/// <summary>
///     Reverse Geocodifica (converte coordenadas para endereço)
/// </summary>
public record ReverseGeocodificarQuery(double Latitude, double Longitude) : IRequest<EnderecoDTO>;