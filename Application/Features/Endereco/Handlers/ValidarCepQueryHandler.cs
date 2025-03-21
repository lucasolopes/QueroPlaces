using Application.Extensions.Common.Models;
using Application.Extensions.Features.Endereco.Queries;
using Application.Extensions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.Endereco.Handlers;

/// <summary>
///     Handler para validação de CEP
/// </summary>
public class ValidarCepQueryHandler : IRequestHandler<ValidarCepQuery, ValidacaoCEPDTO>
{
    private readonly ILogger<ValidarCepQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ValidarCepQueryHandler(IUnitOfWork unitOfWork, ILogger<ValidarCepQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ValidacaoCEPDTO> Handle(ValidarCepQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var cep = request.CEP.Replace("-", "").Trim();
            _logger.LogInformation("Validando CEP: {CEP}", cep);

            // Verificar se o CEP existe
            var existeCEP = await _unitOfWork.CEPs.ExisteCEPAsync(cep, cancellationToken);

            if (!existeCEP)
            {
                _logger.LogInformation("CEP {CEP} não é válido", cep);
                return new ValidacaoCEPDTO
                {
                    Valido = false,
                    Mensagem = $"O CEP {cep} não foi encontrado na base de dados"
                };
            }

            // Obter o tipo do CEP
            var tipoCEP = await _unitOfWork.CEPs.ObterTipoCEPAsync(cep, cancellationToken);

            _logger.LogInformation("CEP {CEP} é válido. Tipo: {TipoCEP}", cep, tipoCEP);
            return new ValidacaoCEPDTO
            {
                Valido = true,
                TipoCEP = tipoCEP,
                Mensagem = $"CEP válido. Tipo: {tipoCEP}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar CEP {CEP}", request.CEP);
            throw;
        }
    }
}