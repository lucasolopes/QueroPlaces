using Application.Extensions.Common.Models;
using Application.Extensions.Extensions.GeoSpatial;
using Application.Extensions.Features.GeoEspacial.Queries;
using Application.Extensions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.GeoEspacial.Handlers;

/// <summary>
///     Handler para busca de endereços por raio
/// </summary>
public class
    BuscarEnderecosPorRaioQueryHandler : IRequestHandler<BuscarEnderecosPorRaioQuery, ResultadoBuscaDTO<EnderecoDTO>>
{
    private readonly ICoordinateConverter _coordinateConverter;
    private readonly ILogger<BuscarEnderecosPorRaioQueryHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public BuscarEnderecosPorRaioQueryHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ICoordinateConverter coordinateConverter,
        ILogger<BuscarEnderecosPorRaioQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _coordinateConverter = coordinateConverter;
        _logger = logger;
    }

    public async Task<ResultadoBuscaDTO<EnderecoDTO>> Handle(BuscarEnderecosPorRaioQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Buscando endereços por raio: CEP={CEP}, Lat={Latitude}, Lng={Longitude}, Raio={RaioKm}km",
                request.CEP, request.Latitude, request.Longitude, request.RaioKm);

            // Variáveis para coordenadas de origem
            double latitudeOrigem;
            double longitudeOrigem;

            // Se temos coordenadas, usá-las
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                latitudeOrigem = request.Latitude.Value;
                longitudeOrigem = request.Longitude.Value;
            }
            // Caso contrário, tentar geocodificar o CEP
            else if (!string.IsNullOrWhiteSpace(request.CEP))
            {
                var geocodificacao = await _mediator.Send(new GeocodificarEnderecoQuery
                {
                    CEP = request.CEP
                }, cancellationToken);

                if (geocodificacao == null || !geocodificacao.Latitude.HasValue || !geocodificacao.Longitude.HasValue)
                    throw new ArgumentException("Não foi possível obter coordenadas a partir do CEP fornecido");

                latitudeOrigem = geocodificacao.Latitude.Value;
                longitudeOrigem = geocodificacao.Longitude.Value;
            }
            else
            {
                throw new ArgumentException("É necessário fornecer CEP ou coordenadas (latitude/longitude)");
            }

            // Criar o ponto de origem
            var pontoOrigem = _coordinateConverter.ToPoint(latitudeOrigem, longitudeOrigem);

            // Em um sistema real com PostGIS, aqui seria feita uma consulta espacial
            // Ex: SELECT * FROM logradouros WHERE ST_DWithin(geom, ST_SetSRID(ST_Point(long, lat), 4326), raioEmMetros)

            // Como estamos simulando, vamos retornar endereços aproximados
            // baseados em uma amostragem da base existente

            // Determinar a UF da origem
            var ufOrigem = DeterminarUFPorCoordenada(latitudeOrigem, longitudeOrigem);

            // Buscar alguns endereços para simular
            List<EnderecoDTO> resultados = new();

            // Determinar qual tipo de entidade buscar
            var tipoEndereco = request.TipoEndereco?.ToLower() ?? "todos";

            // Número de endereços a buscar de cada tipo
            var totalPorTipo = request.TamanhoPagina / (tipoEndereco == "todos" ? 3 : 1);

            // Buscar Localidades
            if (tipoEndereco == "todos" || tipoEndereco == "localidade")
            {
                var localidades = (await _unitOfWork.Localidades.GetAllAsync(cancellationToken))
                    .Where(l => l.UF == ufOrigem)
                    .Take(totalPorTipo)
                    .ToList();

                foreach (var localidade in localidades)
                    resultados.Add(new EnderecoDTO
                    {
                        Localidade = localidade.Nome,
                        UF = localidade.UF,
                        CEP = localidade.CEP,
                        TipoCEP = "LOCALIDADE",
                        // Gerar coordenadas próximas ao ponto de origem
                        Latitude = latitudeOrigem + (new Random().NextDouble() - 0.5) * 0.02 * request.RaioKm,
                        Longitude = longitudeOrigem + (new Random().NextDouble() - 0.5) * 0.02 * request.RaioKm
                    });
            }

            // Buscar Logradouros
            if (tipoEndereco == "todos" || tipoEndereco == "logradouro")
            {
                var logradouros = (await _unitOfWork.Logradouros.GetAllAsync(cancellationToken))
                    .Where(l => l.UF == ufOrigem)
                    .Take(totalPorTipo)
                    .ToList();

                foreach (var logradouro in logradouros)
                    resultados.Add(new EnderecoDTO
                    {
                        TipoLogradouro = logradouro.TipoLogradouro,
                        Logradouro = logradouro.Nome,
                        Complemento = logradouro.Complemento,
                        Bairro = logradouro.BairroInicial?.Nome,
                        Localidade = logradouro.Localidade?.Nome,
                        UF = logradouro.UF,
                        CEP = logradouro.CEP,
                        TipoCEP = "LOGRADOURO",
                        // Gerar coordenadas próximas ao ponto de origem
                        Latitude = latitudeOrigem + (new Random().NextDouble() - 0.5) * 0.02 * request.RaioKm,
                        Longitude = longitudeOrigem + (new Random().NextDouble() - 0.5) * 0.02 * request.RaioKm
                    });
            }

            // Buscar Bairros
            if (tipoEndereco == "todos" || tipoEndereco == "bairro")
            {
                var bairros = (await _unitOfWork.Bairros.GetAllAsync(cancellationToken))
                    .Where(b => b.UF == ufOrigem)
                    .Take(totalPorTipo)
                    .ToList();

                foreach (var bairro in bairros)
                    resultados.Add(new EnderecoDTO
                    {
                        Bairro = bairro.Nome,
                        Localidade = bairro.Localidade?.Nome,
                        UF = bairro.UF,
                        TipoCEP = "BAIRRO",
                        // Gerar coordenadas próximas ao ponto de origem
                        Latitude = latitudeOrigem + (new Random().NextDouble() - 0.5) * 0.02 * request.RaioKm,
                        Longitude = longitudeOrigem + (new Random().NextDouble() - 0.5) * 0.02 * request.RaioKm
                    });
            }

            // Calcular a distância de cada endereço ao ponto de origem
            // e filtrar apenas os que estão dentro do raio solicitado
            var resultadosFiltrados = new List<EnderecoDTO>();

            foreach (var endereco in resultados)
                if (endereco.Latitude.HasValue && endereco.Longitude.HasValue)
                {
                    // Calcular distância
                    var distancia = CalcularDistanciaHaversine(
                        latitudeOrigem, longitudeOrigem,
                        endereco.Latitude.Value, endereco.Longitude.Value);

                    // Adicionar apenas se estiver dentro do raio
                    if (distancia <= request.RaioKm) resultadosFiltrados.Add(endereco);
                }

            // Ordenar por distância (se tivéssemos calculado a distância real)
            // Aqui apenas simulamos a ordenação aleatória
            resultadosFiltrados = resultadosFiltrados
                .OrderBy(e => new Random().Next())
                .Skip((request.Pagina - 1) * request.TamanhoPagina)
                .Take(request.TamanhoPagina)
                .ToList();

            // Montar o resultado
            var resultado = new ResultadoBuscaDTO<EnderecoDTO>
            {
                Resultados = resultadosFiltrados,
                Total = resultadosFiltrados.Count,
                Pagina = request.Pagina,
                TamanhoPagina = request.TamanhoPagina
            };

            _logger.LogInformation("Busca por raio concluída: {Total} resultados encontrados", resultado.Total);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar endereços por raio");
            throw;
        }
    }

    // Método para determinar UF com base nas coordenadas de forma simplificada
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
            var distancia = CalcularDistanciaHaversine(lat, lng, uf.Value.Lat, uf.Value.Lng);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                ufMaisProxima = uf.Key;
            }
        }

        return ufMaisProxima;
    }

    // Método para calcular distância aproximada em km usando a fórmula de Haversine
    private double CalcularDistanciaHaversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Raio da Terra em km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}