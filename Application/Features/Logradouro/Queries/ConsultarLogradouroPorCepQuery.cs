using Application.Extensions.Common.Models;
using MediatR;
using OneOf;

namespace Application.Extensions.Features.Logradouro.Queries;

/// <summary>
///     Consulta logradouros por CEP
/// </summary>
public record ConsultarLogradouroPorCepQuery(string CEP) : IRequest<OneOf<LogradouroDTO, string>>;