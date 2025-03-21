using Application.Extensions.Common.Models;
using Application.Extensions.Features.Endereco.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.Endereco.Handlers;

/// <summary>
///     Handler para autocompletar de endereços
/// </summary>
public class AutocompletarEnderecoQueryHandler : IRequestHandler<AutocompletarEnderecoQuery, IEnumerable<EnderecoDTO>>
{
    private readonly ILogger<AutocompletarEnderecoQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AutocompletarEnderecoQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
        ILogger<AutocompletarEnderecoQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<EnderecoDTO>> Handle(AutocompletarEnderecoQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Executando autocompletar: Termo={Termo}, Tipo={Tipo}, UF={UF}, Limite={Limite}",
                request.Termo, request.Tipo, request.UF, request.Limite);

            var resultados = new List<EnderecoDTO>();

            // Normalizar tipo (se fornecido)
            var tipo = string.IsNullOrWhiteSpace(request.Tipo) ? null : request.Tipo.ToLower();

            // Se não há tipo específico ou tipo é logradouro, buscar logradouros
            if (tipo == null || tipo == "logradouro")
            {
                var logradouros = await _unitOfWork.Logradouros.SearchByNomeAsync(
                    request.Termo,
                    tipo == null ? request.Limite / 3 : request.Limite,
                    cancellationToken);

                // Filtrar por UF se necessário
                if (!string.IsNullOrWhiteSpace(request.UF))
                {
                    var uf = request.UF.ToUpper();
                    logradouros = logradouros.Where(l => l.UF == uf).ToList();
                }

                foreach (var logradouro in logradouros) resultados.Add(_mapper.Map<EnderecoDTO>(logradouro));
            }

            // Se não há tipo específico ou tipo é bairro, buscar bairros
            if (tipo == null || tipo == "bairro")
            {
                var bairros = string.IsNullOrWhiteSpace(request.UF)
                    ? await _unitOfWork.Bairros.SearchByNomeEmQualquerLocalidadeAsync(
                        request.Termo,
                        tipo == null ? request.Limite / 3 : request.Limite,
                        cancellationToken)
                    : (await _unitOfWork.Bairros.GetAllAsync(cancellationToken))
                    .Where(b => b.UF == request.UF.ToUpper() && b.Nome.ToLower().Contains(request.Termo.ToLower()))
                    .Take(tipo == null ? request.Limite / 3 : request.Limite)
                    .ToList();

                foreach (var bairro in bairros)
                    resultados.Add(new EnderecoDTO
                    {
                        Bairro = bairro.Nome,
                        Localidade = bairro.Localidade?.Nome,
                        UF = bairro.UF,
                        TipoCEP = "BAIRRO"
                    });
            }

            // Se não há tipo específico ou tipo é localidade, buscar localidades
            if (tipo == null || tipo == "localidade")
            {
                var localidades = await _unitOfWork.Localidades.SearchByNomeAsync(
                    request.Termo,
                    tipo == null ? request.Limite / 3 : request.Limite,
                    cancellationToken);

                // Filtrar por UF se necessário
                if (!string.IsNullOrWhiteSpace(request.UF))
                {
                    var uf = request.UF.ToUpper();
                    localidades = localidades.Where(l => l.UF == uf).ToList();
                }

                foreach (var localidade in localidades)
                    resultados.Add(new EnderecoDTO
                    {
                        Localidade = localidade.Nome,
                        UF = localidade.UF,
                        CEP = localidade.CEP,
                        TipoCEP = "LOCALIDADE"
                    });
            }

            // Se o tipo não foi especificado, limitar o número total de resultados
            if (tipo == null) resultados = resultados.Take(request.Limite).ToList();

            _logger.LogInformation("Autocompletar concluído: {Quantidade} resultados encontrados", resultados.Count);
            return resultados;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no autocompletar de endereços");
            throw;
        }
    }
}