using System.Text.Json;
using Application.Extensions.Common.Models;
using Application.Extensions.Features.Endereco.Queries;
using Application.Extensions.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OneOf;

namespace Application.Extensions.Features.Endereco.Handlers;

/// <summary>
///     Handler para consulta de endereço por CEP
/// </summary>
public class
    ConsultarEnderecoPorCepQueryHandler : IRequestHandler<ConsultarEnderecoPorCepQuery, OneOf<EnderecoDTO, string>>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<ConsultarEnderecoPorCepQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ConsultarEnderecoPorCepQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ConsultarEnderecoPorCepQueryHandler> logger,
        IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    public async Task<OneOf<EnderecoDTO, string>> Handle(ConsultarEnderecoPorCepQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cep = request.CEP.Replace("-", "").Trim();
            _logger.LogInformation("Consultando endereço pelo CEP: {CEP}", cep);

            // Tentar obter do cache
            var cacheKey = $"endereco:cep:{cep}";
            var cachedEndereco = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedEndereco))
            {
                _logger.LogInformation("Endereço recuperado do cache para o CEP {CEP}", cep);
                return JsonSerializer.Deserialize<EnderecoDTO>(cachedEndereco);
            }

            // Validar o CEP e obter os detalhes
            var resultadoValidacao = await _unitOfWork.CEPs.ValidarCEPAsync(cep, cancellationToken);

            if (resultadoValidacao == null)
            {
                _logger.LogInformation("CEP {CEP} não encontrado na base de dados", cep);
                return $"CEP {cep} não encontrado";
            }

            // Criar o endereço baseado no tipo de resultado
            var endereco = new EnderecoDTO { CEP = cep };
            var tipo = resultadoValidacao.GetType().GetProperty("Tipo")?.GetValue(resultadoValidacao)?.ToString();
            var dados = resultadoValidacao.GetType().GetProperty("Dados")?.GetValue(resultadoValidacao);

            if (dados != null)
                switch (tipo)
                {
                    case "LOGRADOURO":
                        endereco = _mapper.Map<EnderecoDTO>(dados);
                        break;
                    case "LOCALIDADE":
                        endereco = _mapper.Map<EnderecoDTO>(dados);
                        break;
                    case "GRANDE_USUARIO":
                        endereco = _mapper.Map<EnderecoDTO>(dados);
                        break;
                    case "UNIDADE_OPERACIONAL":
                        endereco = _mapper.Map<EnderecoDTO>(dados);
                        break;
                    case "CAIXA_POSTAL_COMUNITARIA":
                        endereco = _mapper.Map<EnderecoDTO>(dados);
                        break;
                    case "FAIXA_UF":
                    case "FAIXA_LOCALIDADE":
                    case "FAIXA_BAIRRO":
                        // Para faixas, tentar obter informações básicas
                        var uf = dados.GetType().GetProperty("UF")?.GetValue(dados)?.ToString();
                        if (uf != null)
                            endereco.UF = uf;

                        // Se for faixa de localidade ou bairro, tentar obter mais informações
                        if (tipo == "FAIXA_LOCALIDADE" || tipo == "FAIXA_BAIRRO")
                        {
                            var localidade = dados.GetType().GetProperty("Localidade")?.GetValue(dados);
                            if (localidade != null)
                                endereco.Localidade = localidade.GetType().GetProperty("Nome")?.GetValue(localidade)
                                    ?.ToString();
                        }

                        if (tipo == "FAIXA_BAIRRO")
                        {
                            var bairro = dados.GetType().GetProperty("Bairro")?.GetValue(dados);
                            if (bairro != null)
                                endereco.Bairro = bairro.GetType().GetProperty("Nome")?.GetValue(bairro)?.ToString();
                        }

                        break;
                }

            endereco.TipoCEP = tipo;

            // Armazenar no cache por 24 horas
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(endereco),
                cacheOptions,
                cancellationToken);

            _logger.LogInformation("Endereço encontrado para o CEP {CEP}: {TipoEndereco}", cep, tipo);
            return endereco;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar endereço pelo CEP {CEP}", request.CEP);
            throw;
        }
    }
}