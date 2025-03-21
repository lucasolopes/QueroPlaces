using Application.Extensions.Common.Models;
using Application.Extensions.Features.Localidade.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.Localidade.Handlers;

/// <summary>
///     Handler para consulta de localidades por UF
/// </summary>
public class
    ConsultarLocalidadePorUFQueryHandler : IRequestHandler<ConsultarLocalidadePorUFQuery, IEnumerable<LocalidadeDTO>>
{
    private readonly ILogger<ConsultarLocalidadePorUFQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ConsultarLocalidadePorUFQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<ConsultarLocalidadePorUFQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<LocalidadeDTO>> Handle(ConsultarLocalidadePorUFQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Consultando localidades da UF: {UF}", request.UF);
            var localidades = await _unitOfWork.Localidades.GetByUFAsync(request.UF, cancellationToken);

            return _mapper.Map<IEnumerable<LocalidadeDTO>>(localidades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar localidades por UF {UF}", request.UF);
            throw;
        }
    }
}