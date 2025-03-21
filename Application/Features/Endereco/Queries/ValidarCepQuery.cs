using Application.Extensions.Common.Models;
using MediatR;

namespace Application.Extensions.Features.Endereco.Queries;

/// <summary>
///     Validação de CEP
/// </summary>
public record ValidarCepQuery(string CEP) : IRequest<ValidacaoCEPDTO>;