using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Logradouro.Queries;

/// <summary>
///     Consulta logradouros por bairro
/// </summary>
public record ConsultarLogradouroPorBairroQuery(int BairroId) : IRequest<IEnumerable<LogradouroDTO>>;