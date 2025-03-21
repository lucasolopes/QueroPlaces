using Application.Extensions.Common.Models;
using Application.Extensions.Extensions.GeoSpatial;
using Application.Extensions.Features.GeoEspacial.Queries;
using Application.Extensions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.GeoEspacial.Handlers;

/// <summary>
///     Handler para calcular distância entre dois pontos ou CEPs
/// </summary>
public class CalcularDistanciaQueryHandler : IRequestHandler<CalcularDistanciaQuery, RespostaDistanciaDTO>
{
    private readonly ICoordinateConverter _coordinateConverter;
    private readonly ILogger<CalcularDistanciaQueryHandler> _logger;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public CalcularDistanciaQueryHandler(
        IUnitOfWork unitOfWork,
        ICoordinateConverter coordinateConverter,
        IMediator mediator,
        ILogger<CalcularDistanciaQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _coordinateConverter = coordinateConverter;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<RespostaDistanciaDTO> Handle(CalcularDistanciaQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Calculando distância entre pontos");

            // Variáveis para armazenar as coordenadas
            var latitudeOrigem = request.LatitudeOrigem;
            var longitudeOrigem = request.LongitudeOrigem;
            var latitudeDestino = request.LatitudeDestino;
            var longitudeDestino = request.LongitudeDestino;
            string? enderecoOrigem = null;
            string? enderecoDestino = null;

            // Se fornecido CEP de origem, obter suas coordenadas
            if (!string.IsNullOrWhiteSpace(request.CEPOrigem))
            {
                _logger.LogInformation("Geocodificando CEP de origem: {CEP}", request.CEPOrigem);
                var geocodificacao = await _mediator.Send(new GeocodificarEnderecoQuery
                {
                    CEP = request.CEPOrigem
                }, cancellationToken);

                if (geocodificacao != null && geocodificacao.Latitude.HasValue && geocodificacao.Longitude.HasValue)
                {
                    latitudeOrigem = geocodificacao.Latitude;
                    longitudeOrigem = geocodificacao.Longitude;
                    enderecoOrigem = FormatarEndereco(geocodificacao);
                }
            }

            // Se fornecido CEP de destino, obter suas coordenadas
            if (!string.IsNullOrWhiteSpace(request.CEPDestino))
            {
                _logger.LogInformation("Geocodificando CEP de destino: {CEP}", request.CEPDestino);
                var geocodificacao = await _mediator.Send(new GeocodificarEnderecoQuery
                {
                    CEP = request.CEPDestino
                }, cancellationToken);

                if (geocodificacao != null && geocodificacao.Latitude.HasValue && geocodificacao.Longitude.HasValue)
                {
                    latitudeDestino = geocodificacao.Latitude;
                    longitudeDestino = geocodificacao.Longitude;
                    enderecoDestino = FormatarEndereco(geocodificacao);
                }
            }

            // Verificar se temos coordenadas completas
            if (!latitudeOrigem.HasValue || !longitudeOrigem.HasValue)
                throw new ArgumentException("Não foi possível obter coordenadas da origem");

            if (!latitudeDestino.HasValue || !longitudeDestino.HasValue)
                throw new ArgumentException("Não foi possível obter coordenadas do destino");

            // Calcular distância diretamente usando o método Haversine
            var distanciaKm = Math.Round(
                _coordinateConverter.CalculateDistance(
                    latitudeOrigem.Value, longitudeOrigem.Value,
                    latitudeDestino.Value, longitudeDestino.Value
                ), 2);

            _logger.LogInformation("Distância calculada: {DistanciaKm} km", distanciaKm);

            // Se não temos o endereço de origem mas temos coordenadas, obter via reverse geocode
            if (string.IsNullOrWhiteSpace(enderecoOrigem) && latitudeOrigem.HasValue && longitudeOrigem.HasValue)
                try
                {
                    var reverseGeocode = await _mediator.Send(new ReverseGeocodificarQuery(
                        latitudeOrigem.Value, longitudeOrigem.Value), cancellationToken);

                    if (reverseGeocode != null) enderecoOrigem = FormatarEndereco(reverseGeocode);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Erro ao realizar reverse geocode para origem. Continuando sem endereço.");
                }

            // Se não temos o endereço de destino mas temos coordenadas, obter via reverse geocode
            if (string.IsNullOrWhiteSpace(enderecoDestino) && latitudeDestino.HasValue && longitudeDestino.HasValue)
                try
                {
                    var reverseGeocode = await _mediator.Send(new ReverseGeocodificarQuery(
                        latitudeDestino.Value, longitudeDestino.Value), cancellationToken);

                    if (reverseGeocode != null) enderecoDestino = FormatarEndereco(reverseGeocode);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Erro ao realizar reverse geocode para destino. Continuando sem endereço.");
                }

            // Retornar resposta
            return new RespostaDistanciaDTO
            {
                DistanciaEmKm = distanciaKm,
                EnderecoOrigem = enderecoOrigem,
                EnderecoDestino = enderecoDestino,
                LatitudeOrigem = latitudeOrigem,
                LongitudeOrigem = longitudeOrigem,
                LatitudeDestino = latitudeDestino,
                LongitudeDestino = longitudeDestino
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular distância");
            throw;
        }
    }

    private string FormatarEndereco(EnderecoDTO endereco)
    {
        var partes = new List<string>();

        if (!string.IsNullOrWhiteSpace(endereco.TipoLogradouro) && !string.IsNullOrWhiteSpace(endereco.Logradouro))
            partes.Add($"{endereco.TipoLogradouro} {endereco.Logradouro}");
        else if (!string.IsNullOrWhiteSpace(endereco.Logradouro))
            partes.Add(endereco.Logradouro);

        if (!string.IsNullOrWhiteSpace(endereco.Complemento))
            partes.Add(endereco.Complemento);

        if (!string.IsNullOrWhiteSpace(endereco.Bairro))
            partes.Add(endereco.Bairro);

        if (!string.IsNullOrWhiteSpace(endereco.Localidade))
            partes.Add(endereco.Localidade);

        if (!string.IsNullOrWhiteSpace(endereco.UF))
            partes.Add(endereco.UF);

        if (!string.IsNullOrWhiteSpace(endereco.CEP))
            partes.Add($"CEP {endereco.CEP}");

        return string.Join(", ", partes);
    }
}