using Application.Extensions.Common.Models;
using Application.Extensions.Features.Bairro.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.Bairro.Handlers;

/// <summary>
///     Handler para pesquisa paginada de bairros
/// </summary>
public class PesquisarBairroQueryHandler : IRequestHandler<PesquisarBairroQuery, ResultadoBuscaDTO<BairroDTO>>
{
    private readonly ILogger<PesquisarBairroQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public PesquisarBairroQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<PesquisarBairroQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResultadoBuscaDTO<BairroDTO>> Handle(PesquisarBairroQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Pesquisando bairros: Nome={Nome}, Localidade={Localidade}, UF={UF}",
                request.Nome, request.Localidade, request.UF);

            // Obter a query base
            var query = (await _unitOfWork.Bairros.GetAllAsync(cancellationToken)).AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                var termo = request.Nome.ToLower();
                query = query.Where(b => b.Nome.ToLower().Contains(termo));
            }

            if (!string.IsNullOrWhiteSpace(request.UF))
            {
                var uf = request.UF.ToUpper();
                query = query.Where(b => b.UF == uf);
            }

            if (!string.IsNullOrWhiteSpace(request.Localidade))
            {
                var termoLocalidade = request.Localidade.ToLower();
                query = query.Where(b => b.Localidade != null && b.Localidade.Nome.ToLower().Contains(termoLocalidade));
            }

            // Ordenar
            query = query
                .OrderBy(b => b.UF)
                .ThenBy(b => b.Localidade != null ? b.Localidade.Nome : "")
                .ThenBy(b => b.Nome);

            // Contar total de registros
            var total = query.Count();

            // Paginar resultados
            var paginados = query
                .Skip((request.Pagina - 1) * request.TamanhoPagina)
                .Take(request.TamanhoPagina)
                .ToList();

            // Mapear para DTOs
            var bairrosDto = _mapper.Map<List<BairroDTO>>(paginados);

            // Montar resultado
            var resultado = new ResultadoBuscaDTO<BairroDTO>
            {
                Resultados = bairrosDto,
                Total = total,
                Pagina = request.Pagina,
                TamanhoPagina = request.TamanhoPagina
            };

            _logger.LogInformation("Pesquisa de bairros concluída: {Total} resultados encontrados", total);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao pesquisar bairros");
            throw;
        }
    }
}