using System.Text.Json;
using Application.Extensions.Common.Models;
using Application.Extensions.Features.Endereco.Queries;
using Application.Extensions.Features.GeoEspacial.Queries;
using Application.Extensions.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.GeoEspacial.Handlers;

/// <summary>
///     Handler para geocodificação de endereços
/// </summary>
public class GeocodificarEnderecoQueryHandler : IRequestHandler<GeocodificarEnderecoQuery, EnderecoDTO>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<GeocodificarEnderecoQueryHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public GeocodificarEnderecoQueryHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<GeocodificarEnderecoQueryHandler> logger,
        IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
        _cache = cache;
    }

    public async Task<EnderecoDTO> Handle(GeocodificarEnderecoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Geocodificando endereço: CEP={CEP}, Logradouro={Logradouro}, Localidade={Localidade}, UF={UF}",
                request.CEP, request.Logradouro, request.Localidade, request.UF);

            // Verificar se temos o CEP
            if (!string.IsNullOrWhiteSpace(request.CEP))
            {
                // Tentar obter do cache primeiro
                var cacheKey = $"geocode:cep:{request.CEP}";
                var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

                if (!string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation("Geocodificação recuperada do cache para o CEP {CEP}", request.CEP);
                    var cachedEndereco = JsonSerializer.Deserialize<EnderecoDTO>(cachedData);

                    // Se já temos coordenadas no cache, retornar diretamente
                    if (cachedEndereco?.Latitude.HasValue == true && cachedEndereco?.Longitude.HasValue == true)
                        return cachedEndereco;
                }

                // Consultar o endereço pelo CEP
                var result = await _mediator.Send(new ConsultarEnderecoPorCepQuery(request.CEP), cancellationToken);

                if (result.IsT0) // EnderecoDTO
                {
                    var endereco = result.AsT0;

                    // Verificar se já tem coordenadas
                    if (!endereco.Latitude.HasValue || !endereco.Longitude.HasValue)
                    {
                        // Geocodificar o endereço utilizando um serviço de geocodificação
                        // Aqui usaremos coordenadas aproximadas baseadas na UF como exemplo
                        // Em produção, deve-se implementar uma integração com API de geocodificação
                        (endereco.Latitude, endereco.Longitude) = ObterCoordenadaAproximada(endereco);

                        // Salvar no cache
                        var cacheOptions = new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) // CEPs mudam raramente
                        };

                        await _cache.SetStringAsync(
                            cacheKey,
                            JsonSerializer.Serialize(endereco),
                            cacheOptions,
                            cancellationToken);
                    }

                    return endereco;
                }
            }

            // Se não temos CEP ou não encontrou pelo CEP, tenta pelos demais dados
            if (!string.IsNullOrWhiteSpace(request.Logradouro) &&
                !string.IsNullOrWhiteSpace(request.Localidade) &&
                !string.IsNullOrWhiteSpace(request.UF))
            {
                // Construir endereço baseado nos dados fornecidos
                var endereco = new EnderecoDTO
                {
                    TipoLogradouro = "", // Não temos informação aqui
                    Logradouro = request.Logradouro,
                    Complemento = request.Complemento,
                    Bairro = request.Bairro,
                    Localidade = request.Localidade,
                    UF = request.UF.ToUpper(),
                    CEP = request.CEP,
                    TipoCEP = "APROXIMADO"
                };

                // Geocodificar utilizando serviço de geocodificação
                (endereco.Latitude, endereco.Longitude) = ObterCoordenadaAproximada(endereco);

                return endereco;
            }

            // Se chegou até aqui é porque não temos dados suficientes
            throw new ArgumentException(
                "Dados insuficientes para geocodificação. Forneça CEP ou (Logradouro, Localidade e UF)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao geocodificar endereço");
            throw;
        }
    }

    // Método para obter coordenadas aproximadas por UF
    // Em ambiente de produção, deve-se substituir por uma integração real com API de geocodificação
    private (double Latitude, double Longitude) ObterCoordenadaAproximada(EnderecoDTO endereco)
    {
        // Coordenadas aproximadas das capitais por UF
        var coordenadasPorUF = new Dictionary<string, (double Lat, double Lng)>
        {
            { "AC", (-9.97499, -67.82462) },
            { "AL", (-9.64706, -35.73309) },
            { "AM", (-3.10719, -60.02613) },
            { "AP", (0.03861, -51.05653) },
            { "BA", (-12.97111, -38.51083) },
            { "CE", (-3.71839, -38.54343) },
            { "DF", (-15.77972, -47.92972) },
            { "ES", (-20.31778, -40.33778) },
            { "GO", (-16.67861, -49.25389) },
            { "MA", (-2.53073, -44.30278) },
            { "MG", (-19.92083, -43.93778) },
            { "MS", (-20.44278, -54.64722) },
            { "MT", (-15.59889, -56.09667) },
            { "PA", (-1.45583, -48.50444) },
            { "PB", (-7.11528, -34.86306) },
            { "PE", (-8.05389, -34.88111) },
            { "PI", (-5.09194, -42.80333) },
            { "PR", (-25.42778, -49.27306) },
            { "RJ", (-22.90278, -43.20722) },
            { "RN", (-5.79500, -35.20944) },
            { "RO", (-8.76194, -63.90389) },
            { "RR", (2.82056, -60.67333) },
            { "RS", (-30.03306, -51.23000) },
            { "SC", (-27.59667, -48.54917) },
            { "SE", (-10.91111, -37.07167) },
            { "SP", (-23.55052, -46.63331) },
            { "TO", (-10.18889, -48.33361) }
        };

        // Adicionar um pequeno deslocamento aleatório (até 5km) para evitar coordenadas idênticas
        var random = new Random();
        var offsetLat = (random.NextDouble() - 0.5) * 0.09; // aproximadamente ±5km
        var offsetLng = (random.NextDouble() - 0.5) * 0.09; // aproximadamente ±5km

        if (!string.IsNullOrEmpty(endereco.UF) && coordenadasPorUF.TryGetValue(endereco.UF, out var coord))
            return (coord.Lat + offsetLat, coord.Lng + offsetLng);

        // Default para o centro do Brasil se UF desconhecida
        return (-15.77972 + offsetLat, -47.92972 + offsetLng);
    }
}