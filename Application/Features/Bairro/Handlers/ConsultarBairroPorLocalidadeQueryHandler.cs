using Application.Extensions.Common.Models;
using Application.Extensions.Features.Bairro.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.Bairro.Handlers;

/// <summary>
///     Handler para consulta de bairros por localidade
/// </summary>
public class
    ConsultarBairroPorLocalidadeQueryHandler : IRequestHandler<ConsultarBairroPorLocalidadeQuery,
    IEnumerable<BairroDTO>>
{
    private readonly ILogger<ConsultarBairroPorLocalidadeQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ConsultarBairroPorLocalidadeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<ConsultarBairroPorLocalidadeQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<BairroDTO>> Handle(ConsultarBairroPorLocalidadeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Consultando bairros da localidade: ID {LocalidadeId}", request.LocalidadeId);

            var bairros = await _unitOfWork.Bairros.GetByLocalidadeIdAsync(request.LocalidadeId, cancellationToken);

            return _mapper.Map<IEnumerable<BairroDTO>>(bairros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar bairros por localidade ID {LocalidadeId}", request.LocalidadeId);
            throw;
        }
    }
}