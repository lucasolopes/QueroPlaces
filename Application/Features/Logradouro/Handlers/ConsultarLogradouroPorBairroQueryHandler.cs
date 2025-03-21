using Application.Extensions.Common.Models;
using Application.Extensions.Features.Logradouro.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.Logradouro.Handlers;

/// <summary>
///     Handler para consulta de logradouros por bairro
/// </summary>
public class
    ConsultarLogradouroPorBairroQueryHandler : IRequestHandler<ConsultarLogradouroPorBairroQuery,
    IEnumerable<LogradouroDTO>>
{
    private readonly ILogger<ConsultarLogradouroPorBairroQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ConsultarLogradouroPorBairroQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<ConsultarLogradouroPorBairroQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<LogradouroDTO>> Handle(ConsultarLogradouroPorBairroQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Consultando logradouros do bairro: ID {BairroId}", request.BairroId);

            var logradouros = await _unitOfWork.Logradouros.GetByBairroIdAsync(request.BairroId, cancellationToken);

            return _mapper.Map<IEnumerable<LogradouroDTO>>(logradouros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar logradouros por bairro ID {BairroId}", request.BairroId);
            throw;
        }
    }
}

/// <summary>
///     Handler para pesquisa paginada de logradouros
/// </summary>
public class
    PesquisarLogradouroQueryHandler : IRequestHandler<PesquisarLogradouroQuery, ResultadoBuscaDTO<LogradouroDTO>>
{
    private readonly ILogger<PesquisarLogradouroQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public PesquisarLogradouroQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<PesquisarLogradouroQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResultadoBuscaDTO<LogradouroDTO>> Handle(PesquisarLogradouroQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Pesquisando logradouros: Nome={Nome}, TipoLogradouro={TipoLogradouro}, Bairro={Bairro}, Localidade={Localidade}, UF={UF}, CEP={CEP}",
                request.Nome, request.TipoLogradouro, request.Bairro, request.Localidade, request.UF, request.CEP);

            // Obter a query base
            var query = (await _unitOfWork.Logradouros.GetAllAsync(cancellationToken)).AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(request.Nome))
            {
                var termo = request.Nome.ToLower();
                query = query.Where(l => l.Nome.ToLower().Contains(termo));
            }

            if (!string.IsNullOrWhiteSpace(request.TipoLogradouro))
            {
                var tipoLogradouro = request.TipoLogradouro.ToLower();
                query = query.Where(l => l.TipoLogradouro.ToLower().Contains(tipoLogradouro));
            }

            if (!string.IsNullOrWhiteSpace(request.Bairro))
            {
                var termoBairro = request.Bairro.ToLower();
                query = query.Where(l =>
                    (l.BairroInicial != null && l.BairroInicial.Nome.ToLower().Contains(termoBairro)) ||
                    (l.BairroFinal != null && l.BairroFinal.Nome.ToLower().Contains(termoBairro)));
            }

            if (!string.IsNullOrWhiteSpace(request.Localidade))
            {
                var termoLocalidade = request.Localidade.ToLower();
                query = query.Where(l => l.Localidade != null && l.Localidade.Nome.ToLower().Contains(termoLocalidade));
            }

            if (!string.IsNullOrWhiteSpace(request.UF))
            {
                // Normalizar UF para maiúsculas
                var uf = request.UF.ToUpper();
                query = query.Where(l => l.UF == uf);
            }

            if (!string.IsNullOrWhiteSpace(request.CEP)) query = query.Where(l => l.CEP == request.CEP);

            // Ordenar
            query = query
                .OrderBy(l => l.UF)
                .ThenBy(l => l.LocalidadeId)
                .ThenBy(l => l.Nome);

            // Contar total de registros
            var total = query.Count();

            // Paginar resultados
            var paginados = query
                .Skip((request.Pagina - 1) * request.TamanhoPagina)
                .Take(request.TamanhoPagina)
                .ToList();

            // Mapear para DTOs
            var logradourosDto = _mapper.Map<List<LogradouroDTO>>(paginados);

            // Montar resultado
            var resultado = new ResultadoBuscaDTO<LogradouroDTO>
            {
                Resultados = logradourosDto,
                Total = total,
                Pagina = request.Pagina,
                TamanhoPagina = request.TamanhoPagina
            };

            _logger.LogInformation("Pesquisa de logradouros concluída: {Total} resultados encontrados", total);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao pesquisar logradouros");
            throw;
        }
    }
}