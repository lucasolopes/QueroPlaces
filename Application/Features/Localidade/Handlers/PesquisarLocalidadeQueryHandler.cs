using Application.Extensions.Common.Models;
using Application.Extensions.Features.Localidade.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.Localidade.Handlers;

/// <summary>
///     Handler para pesquisa paginada de localidades
/// </summary>
public class
    PesquisarLocalidadeQueryHandler : IRequestHandler<PesquisarLocalidadeQuery, ResultadoBuscaDTO<LocalidadeDTO>>
{
    private readonly ILogger<PesquisarLocalidadeQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public PesquisarLocalidadeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<PesquisarLocalidadeQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResultadoBuscaDTO<LocalidadeDTO>> Handle(PesquisarLocalidadeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Pesquisando localidades: Nome={Nome}, UF={UF}, CodigoIBGE={CodigoIBGE}",
                request.Nome, request.UF, request.CodigoIBGE);

            // Obter a query base
            var query = (await _unitOfWork.Localidades.GetAllAsync(cancellationToken)).AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                var termo = request.Nome.ToLower();
                query = query.Where(l => l.Nome.ToLower().Contains(termo));
            }

            if (!string.IsNullOrWhiteSpace(request.UF))
            {
                var uf = request.UF.ToUpper();
                query = query.Where(l => l.UF == uf);
            }

            if (request.CodigoIBGE.HasValue) query = query.Where(l => l.CodigoIBGE == request.CodigoIBGE.Value);

            // Ordenar e paginar
            query = query.OrderBy(l => l.UF).ThenBy(l => l.Nome);

            // Contar total de registros
            var total = query.Count();

            // Paginar resultados
            var paginados = query
                .Skip((request.Pagina - 1) * request.TamanhoPagina)
                .Take(request.TamanhoPagina)
                .ToList();

            // Mapear para DTOs
            var localidadesDto = _mapper.Map<List<LocalidadeDTO>>(paginados);

            // Montar resultado
            var resultado = new ResultadoBuscaDTO<LocalidadeDTO>
            {
                Resultados = localidadesDto,
                Total = total,
                Pagina = request.Pagina,
                TamanhoPagina = request.TamanhoPagina
            };

            _logger.LogInformation("Pesquisa de localidades concluída: {Total} resultados encontrados", total);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao pesquisar localidades");
            throw;
        }
    }
}