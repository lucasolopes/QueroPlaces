using Application.Extensions.Common.Models;
using Application.Extensions.Features.Bairro.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace Application.Extensions.Features.Bairro.Handlers;

/// <summary>
///     Handler para consulta de bairro por ID
/// </summary>
public class ConsultarBairroPorIdQueryHandler : IRequestHandler<ConsultarBairroPorIdQuery, OneOf<BairroDTO, string>>
{
    private readonly ILogger<ConsultarBairroPorIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ConsultarBairroPorIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<ConsultarBairroPorIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OneOf<BairroDTO, string>> Handle(ConsultarBairroPorIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var bairro = await _unitOfWork.Bairros.GetByIdAsync(request.Id, cancellationToken);

            if (bairro == null)
            {
                _logger.LogInformation("Bairro não encontrado: ID {Id}", request.Id);
                return $"Bairro com ID {request.Id} não encontrado";
            }

            _logger.LogInformation("Bairro encontrado: {Nome} ({UF})", bairro.Nome, bairro.UF);
            return _mapper.Map<BairroDTO>(bairro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar bairro por ID {Id}", request.Id);
            throw;
        }
    }
}