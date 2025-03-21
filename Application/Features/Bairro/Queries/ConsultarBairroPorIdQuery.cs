using Application.Extensions.Common.Models;
using MediatR;
using OneOf;

namespace Application.Extensions.Features.Bairro.Queries;

/// <summary>
///     Consulta um bairro por ID
/// </summary>
public record ConsultarBairroPorIdQuery(int Id) : IRequest<OneOf<BairroDTO, string>>;