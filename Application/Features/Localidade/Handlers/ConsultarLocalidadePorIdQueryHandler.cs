using Application.Extensions.Common.Models;
using Application.Extensions.Features.Localidade.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace Application.Extensions.Features.Localidade.Handlers;

/// <summary>
///     Handler para consulta de localidade por ID
/// </summary>
public class
    ConsultarLocalidadePorIdQueryHandler : IRequestHandler<ConsultarLocalidadePorIdQuery, OneOf<LocalidadeDTO, string>>
{
    private readonly ILogger<ConsultarLocalidadePorIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ConsultarLocalidadePorIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<ConsultarLocalidadePorIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OneOf<LocalidadeDTO, string>> Handle(ConsultarLocalidadePorIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var localidade = await _unitOfWork.Localidades.GetByIdAsync(request.Id, cancellationToken);

            if (localidade == null)
            {
                _logger.LogInformation("Localidade não encontrada: ID {Id}", request.Id);
                return $"Localidade com ID {request.Id} não encontrada";
            }

            _logger.LogInformation("Localidade encontrada: {Nome} ({UF})", localidade.Nome, localidade.UF);
            return _mapper.Map<LocalidadeDTO>(localidade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar localidade por ID {Id}", request.Id);
            throw;
        }
    }
}