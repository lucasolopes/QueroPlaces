using System.Text.Json;
using Application.Extensions.Common.Models;
using Application.Extensions.Extensions.GeoSpatial;
using Application.Extensions.Features.GeoEspacial.Queries;
using Application.Extensions.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Application.Extensions.Features.GeoEspacial.Handlers;

/// <summary>
///     Handler para reverse geocodificação (coordenadas para endereço)
/// </summary>
public class ReverseGeocodificarQueryHandler : IRequestHandler<ReverseGeocodificarQuery, EnderecoDTO>
{
    private readonly IDistributedCache _cache;
    private readonly ICoordinateConverter _coordinateConverter;
    private readonly ILogger<ReverseGeocodificarQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ReverseGeocodificarQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<ReverseGeocodificarQueryHandler> logger,
        ICoordinateConverter coordinateConverter,
        IDistributedCache cache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _coordinateConverter = coordinateConverter;
        _cache = cache;
    }

    public async Task<EnderecoDTO> Handle(ReverseGeocodificarQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Executando reverse geocodificação: Lat={Latitude}, Lng={Longitude}",
                request.Latitude, request.Longitude);

            // Arredondar coordenadas para 5 casas decimais para cache (aproximadamente 1m de precisão)
            var latRounded = Math.Round(request.Latitude, 5);
            var lngRounded = Math.Round(request.Longitude, 5);

            // Verificar cache primeiro
            var cacheKey = $"revgeo:{latRounded}:{lngRounded}";
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Reverse geocode recuperado do cache para Lat={Latitude}, Lng={Longitude}",
                    latRounded, lngRounded);
                return JsonSerializer.Deserialize<EnderecoDTO>(cachedData);
            }

            // Criar o ponto com as coordenadas
            var ponto = _coordinateConverter.ToPoint(request.Latitude, request.Longitude);

            // Em um sistema real, aqui seria feita uma consulta espacial ao banco de dados
            // usando PostGIS para encontrar o logradouro/bairro/localidade mais próximo
            // Como estamos simulando, vamos retornar informações aproximadas baseadas na UF

            // A UF pode ser determinada aproximadamente com base nas coordenadas
            var uf = DeterminarUFPorCoordenada(request.Latitude, request.Longitude);

            // Construir uma resposta aproximada
            var endereco = new EnderecoDTO
            {
                Logradouro = "Endereço aproximado",
                Bairro = "Bairro aproximado",
                Localidade = "Localidade aproximada",
                UF = uf,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                TipoCEP = "APROXIMADO"
            };

            // Salvar no cache por 30 dias (coordenadas geográficas são estáveis)
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(endereco),
                cacheOptions,
                cancellationToken);

            _logger.LogInformation("Reverse geocode concluído para Lat={Latitude}, Lng={Longitude}: UF={UF}",
                request.Latitude, request.Longitude, endereco.UF);

            return endereco;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na reverse geocodificação");
            throw;
        }
    }

    // Método para determinar UF com base nas coordenadas de forma simplificada
    // Em ambiente de produção, deve-se usar uma consulta espacial ao banco com PostGIS
    private string DeterminarUFPorCoordenada(double lat, double lng)
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

        // Encontrar a UF mais próxima das coordenadas fornecidas
        var menorDistancia = double.MaxValue;
        var ufMaisProxima = "DF"; // Default para o centro do Brasil

        foreach (var uf in coordenadasPorUF)
        {
            var distancia =
                _coordinateConverter.CalculateDistance(lat, lng, uf.Value.Lat, uf.Value.Lng) /
                1000; // Converter para km
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                ufMaisProxima = uf.Key;
            }
        }

        return ufMaisProxima;
    }
}