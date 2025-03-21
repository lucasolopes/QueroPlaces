using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Localidade.Queries;

/// <summary>
///     Consulta localidades por UF
/// </summary>
public record ConsultarLocalidadePorUFQuery : IRequest<IEnumerable<LocalidadeDTO>>
{
    public string UF { get; init; } = null!;
}