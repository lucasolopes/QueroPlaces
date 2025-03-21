using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Logradouro.Queries;

/// <summary>
///     Consulta logradouros por localidade
/// </summary>
public record ConsultarLogradouroPorLocalidadeQuery(int LocalidadeId) : IRequest<IEnumerable<LogradouroDTO>>;