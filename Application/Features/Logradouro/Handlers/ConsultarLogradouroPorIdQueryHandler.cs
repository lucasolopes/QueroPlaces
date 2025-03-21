using Application.Extensions.Common.Models;
using Application.Extensions.Features.Logradouro.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace Application.Extensions.Features.Logradouro.Handlers;

/// <summary>
///     Handler para consulta de logradouro por ID
/// </summary>
public class
    ConsultarLogradouroPorIdQueryHandler : IRequestHandler<ConsultarLogradouroPorIdQuery, OneOf<LogradouroDTO, string>>
{
    private readonly ILogger<ConsultarLogradouroPorIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ConsultarLogradouroPorIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<ConsultarLogradouroPorIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OneOf<LogradouroDTO, string>> Handle(ConsultarLogradouroPorIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var logradouro = await _unitOfWork.Logradouros.GetByIdAsync(request.Id, cancellationToken);

            if (logradouro == null)
            {
                _logger.LogInformation("Logradouro não encontrado: ID {Id}", request.Id);
                return $"Logradouro com ID {request.Id} não encontrado";
            }

            _logger.LogInformation("Logradouro encontrado: {TipoLogradouro} {Nome} ({UF})",
                logradouro.TipoLogradouro, logradouro.Nome, logradouro.UF);

            return _mapper.Map<LogradouroDTO>(logradouro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar logradouro por ID {Id}", request.Id);
            throw;
        }
    }
}