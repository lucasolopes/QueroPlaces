using Application.Extensions.Common.Models;
using Application.Extensions.Features.Logradouro.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.Logradouro.Handlers;

/// <summary>
///     Handler para consulta de logradouros por localidade
/// </summary>
public class
    ConsultarLogradouroPorLocalidadeQueryHandler : IRequestHandler<ConsultarLogradouroPorLocalidadeQuery,
    IEnumerable<LogradouroDTO>>
{
    private readonly ILogger<ConsultarLogradouroPorLocalidadeQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ConsultarLogradouroPorLocalidadeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<ConsultarLogradouroPorLocalidadeQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<LogradouroDTO>> Handle(ConsultarLogradouroPorLocalidadeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Consultando logradouros da localidade: ID {LocalidadeId}", request.LocalidadeId);

            var logradouros =
                await _unitOfWork.Logradouros.GetByLocalidadeIdAsync(request.LocalidadeId, cancellationToken);

            return _mapper.Map<IEnumerable<LogradouroDTO>>(logradouros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar logradouros por localidade ID {LocalidadeId}",
                request.LocalidadeId);
            throw;
        }
    }
}