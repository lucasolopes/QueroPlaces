using Application.Extensions.Common.Models;
using MediatR;
using OneOf;

namespace Application.Extensions.Features.Logradouro.Queries;

/// <summary>
///     Consulta um logradouro por ID
/// </summary>
public record ConsultarLogradouroPorIdQuery(int Id) : IRequest<OneOf<LogradouroDTO, string>>;