using Application.Extensions.Common.Models;
using Application.Extensions.Features.Endereco.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.Endereco.Handlers;

/// <summary>
///     Handler para pesquisa de endereços
/// </summary>
public class PesquisarEnderecoQueryHandler : IRequestHandler<PesquisarEnderecoQuery, ResultadoBuscaDTO<EnderecoDTO>>
{
    private readonly ILogger<PesquisarEnderecoQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public PesquisarEnderecoQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator,
        ILogger<PesquisarEnderecoQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<ResultadoBuscaDTO<EnderecoDTO>> Handle(PesquisarEnderecoQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Pesquisando endereços: CEP={CEP}, Logradouro={Logradouro}, Bairro={Bairro}, Localidade={Localidade}, UF={UF}",
                request.CEP, request.Logradouro, request.Bairro, request.Localidade, request.UF);

            // Lista para armazenar os resultados
            var resultados = new List<EnderecoDTO>();
            var totalRegistros = 0;

            // Verificar se temos um CEP específico
            if (!string.IsNullOrWhiteSpace(request.CEP))
            {
                var validacaoCEP = await _unitOfWork.CEPs.ValidarCEPAsync(request.CEP, cancellationToken);
                if (validacaoCEP != null)
                {
                    // Usar o mediator para chamar o outro handler
                    var endereco =
                        await _mediator.Send(new ConsultarEnderecoPorCepQuery(request.CEP), cancellationToken);

                    if (endereco.IsT0) // Se for um EnderecoDTO
                    {
                        resultados.Add(endereco.AsT0);
                        totalRegistros = 1;
                    }
                }
            }
            else
            {
                // Definir a ordem de prioridade de busca
                // 1. Logradouros - mais específicos
                if (!string.IsNullOrWhiteSpace(request.Logradouro))
                {
                    var query = (await _unitOfWork.Logradouros.GetAllAsync(cancellationToken)).AsQueryable();

                    // Aplicar filtros
                    var termoLogradouro = request.Logradouro.ToLower();
                    query = query.Where(l => l.Nome.ToLower().Contains(termoLogradouro));

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
                        query = query.Where(l =>
                            l.Localidade != null && l.Localidade.Nome.ToLower().Contains(termoLocalidade));
                    }

                    if (!string.IsNullOrWhiteSpace(request.UF))
                    {
                        var uf = request.UF.ToUpper();
                        query = query.Where(l => l.UF == uf);
                    }

                    // Contar total
                    totalRegistros = query.Count();

                    // Paginar
                    var logradouros = query
                        .Skip((request.Pagina - 1) * request.TamanhoPagina)
                        .Take(request.TamanhoPagina)
                        .ToList();

                    // Mapear para EnderecoDTO
                    foreach (var logradouro in logradouros) resultados.Add(_mapper.Map<EnderecoDTO>(logradouro));
                }
                // 2. Bairros - médio nível de especificidade
                else if (!string.IsNullOrWhiteSpace(request.Bairro))
                {
                    var query = (await _unitOfWork.Bairros.GetAllAsync(cancellationToken)).AsQueryable();

                    // Aplicar filtros
                    var termoBairro = request.Bairro.ToLower();
                    query = query.Where(b => b.Nome.ToLower().Contains(termoBairro));

                    if (!string.IsNullOrWhiteSpace(request.Localidade))
                    {
                        var termoLocalidade = request.Localidade.ToLower();
                        query = query.Where(b =>
                            b.Localidade != null && b.Localidade.Nome.ToLower().Contains(termoLocalidade));
                    }

                    if (!string.IsNullOrWhiteSpace(request.UF))
                    {
                        var uf = request.UF.ToUpper();
                        query = query.Where(b => b.UF == uf);
                    }

                    // Contar total
                    totalRegistros = query.Count();

                    // Paginar
                    var bairros = query
                        .Skip((request.Pagina - 1) * request.TamanhoPagina)
                        .Take(request.TamanhoPagina)
                        .ToList();

                    // Criar EnderecoDTO para cada bairro
                    foreach (var bairro in bairros)
                        resultados.Add(new EnderecoDTO
                        {
                            Bairro = bairro.Nome,
                            Localidade = bairro.Localidade?.Nome,
                            UF = bairro.UF,
                            TipoCEP = "BAIRRO"
                        });
                }
                // 3. Localidades - menor nível de especificidade
                else if (!string.IsNullOrWhiteSpace(request.Localidade) || !string.IsNullOrWhiteSpace(request.UF))
                {
                    var query = (await _unitOfWork.Localidades.GetAllAsync(cancellationToken)).AsQueryable();

                    // Aplicar filtros
                    if (!string.IsNullOrWhiteSpace(request.Localidade))
                    {
                        var termoLocalidade = request.Localidade.ToLower();
                        query = query.Where(l => l.Nome.ToLower().Contains(termoLocalidade));
                    }

                    if (!string.IsNullOrWhiteSpace(request.UF))
                    {
                        var uf = request.UF.ToUpper();
                        query = query.Where(l => l.UF == uf);
                    }

                    // Contar total
                    totalRegistros = query.Count();

                    // Paginar
                    var localidades = query
                        .Skip((request.Pagina - 1) * request.TamanhoPagina)
                        .Take(request.TamanhoPagina)
                        .ToList();

                    // Mapear para EnderecoDTO
                    foreach (var localidade in localidades)
                        resultados.Add(new EnderecoDTO
                        {
                            Localidade = localidade.Nome,
                            UF = localidade.UF,
                            CEP = localidade.CEP,
                            TipoCEP = "LOCALIDADE"
                        });
                }
            }

            // Montar resultado
            var resultado = new ResultadoBuscaDTO<EnderecoDTO>
            {
                Resultados = resultados,
                Total = totalRegistros,
                Pagina = request.Pagina,
                TamanhoPagina = request.TamanhoPagina
            };

            _logger.LogInformation("Pesquisa de endereços concluída: {Total} resultados encontrados", totalRegistros);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao pesquisar endereços");
            throw;
        }
    }
}