using Application.Extensions.Common.Models;
using MediatR;
using OneOf;

namespace Application.Extensions.Features.Localidade.Queries;

/// <summary>
///     Consulta uma localidade por ID
/// </summary>
public record ConsultarLocalidadePorIdQuery(int Id) : IRequest<OneOf<LocalidadeDTO, string>>;