using Application.Extensions.Common.Models;
using MediatR;
using OneOf;

namespace Application.Extensions.Features.Endereco.Queries;

/// <summary>
///     Consulta um endereço pelo CEP
/// </summary>
public record ConsultarEnderecoPorCepQuery(string CEP) : IRequest<OneOf<EnderecoDTO, string>>;