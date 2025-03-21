using Application.Extensions.Common.Models;
using Application.Extensions.Features.Logradouro.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace Application.Extensions.Features.Logradouro.Handlers;

/// <summary>
///     Handler para consulta de logradouro por CEP
/// </summary>
public class
    ConsultarLogradouroPorCepQueryHandler : IRequestHandler<ConsultarLogradouroPorCepQuery,
    OneOf<LogradouroDTO, string>>
{
    private readonly ILogger<ConsultarLogradouroPorCepQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ConsultarLogradouroPorCepQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<ConsultarLogradouroPorCepQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OneOf<LogradouroDTO, string>> Handle(ConsultarLogradouroPorCepQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Consultando logradouro pelo CEP: {CEP}", request.CEP);

            var logradouro = await _unitOfWork.Logradouros.GetByCEPAsync(request.CEP, cancellationToken);

            if (logradouro == null)
            {
                _logger.LogInformation("Logradouro não encontrado pelo CEP: {CEP}", request.CEP);
                return $"Logradouro com CEP {request.CEP} não encontrado";
            }

            _logger.LogInformation("Logradouro encontrado pelo CEP {CEP}: {TipoLogradouro} {Nome} ({UF})",
                request.CEP, logradouro.TipoLogradouro, logradouro.Nome, logradouro.UF);

            return _mapper.Map<LogradouroDTO>(logradouro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar logradouro pelo CEP {CEP}", request.CEP);
            throw;
        }
    }
}