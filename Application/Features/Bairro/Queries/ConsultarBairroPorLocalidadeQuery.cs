using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Bairro.Queries;

/// <summary>
///     Consulta bairros por localidade
/// </summary>
public record ConsultarBairroPorLocalidadeQuery(int LocalidadeId) : IRequest<IEnumerable<BairroDTO>>;