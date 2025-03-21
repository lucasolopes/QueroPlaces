using Application.Extensions.Common.Models;
using Application.Extensions.Features.GeoEspacial.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace QueroPlaces.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GeoEspacialController : ControllerBase
{
    private readonly IMediator _mediator;

    public GeoEspacialController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Calcula a distância entre dois CEPs ou coordenadas
    /// </summary>
    /// <param name="request">Parâmetros com origem e destino</param>
    /// <returns>Distância calculada e informações dos endereços</returns>
    [HttpPost("calcular-distancia")]
    [ProducesResponseType(typeof(RespostaDistanciaDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalcularDistancia([FromBody] CalcularDistanciaQuery request)
    {
        // Validar se pelo menos um par de identificação foi fornecido
        if (string.IsNullOrEmpty(request.CEPOrigem) &&
            (request.LatitudeOrigem == null || request.LongitudeOrigem == null))
            return BadRequest("É necessário informar CEP de origem ou coordenadas de origem.");

        if (string.IsNullOrEmpty(request.CEPDestino) &&
            (request.LatitudeDestino == null || request.LongitudeDestino == null))
            return BadRequest("É necessário informar CEP de destino ou coordenadas de destino.");

        var resultado = await _mediator.Send(request);
        return Ok(resultado);
    }

    /// <summary>
    ///     Busca endereços próximos a um ponto por raio
    /// </summary>
    /// <param name="request">Parâmetros da busca</param>
    /// <returns>Lista de endereços dentro do raio especificado</returns>
    [HttpGet("buscar-por-raio")]
    [ProducesResponseType(typeof(ResultadoBuscaDTO<EnderecoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BuscarPorRaio([FromQuery] BuscarEnderecosPorRaioQuery request)
    {
        // Validar se origem foi informada
        if (string.IsNullOrEmpty(request.CEP) && (request.Latitude == null || request.Longitude == null))
            return BadRequest("É necessário informar CEP ou coordenadas (latitude/longitude) de origem.");

        var resultado = await _mediator.Send(request);
        return Ok(resultado);
    }

    /// <summary>
    ///     Geocodifica um endereço (converte endereço para coordenadas)
    /// </summary>
    /// <param name="request">Dados do endereço a ser geocodificado</param>
    /// <returns>Endereço com coordenadas</returns>
    [HttpPost("geocodificar")]
    [ProducesResponseType(typeof(EnderecoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Geocodificar([FromBody] GeocodificarEnderecoQuery request)
    {
        // Validar se há informações suficientes
        if (string.IsNullOrEmpty(request.CEP) &&
            (string.IsNullOrEmpty(request.Logradouro) ||
             string.IsNullOrEmpty(request.Localidade) ||
             string.IsNullOrEmpty(request.UF)))
            return BadRequest("É necessário informar CEP ou (Logradouro, Localidade e UF).");

        var resultado = await _mediator.Send(request);
        return Ok(resultado);
    }

    /// <summary>
    ///     Reverse Geocodifica (converte coordenadas para endereço)
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <returns>Endereço correspondente às coordenadas</returns>
    [HttpGet("reverse-geocodificar")]
    [ProducesResponseType(typeof(EnderecoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReverseGeocodificar([FromQuery] double latitude, [FromQuery] double longitude)
    {
        // Validar as coordenadas
        if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
            return BadRequest(
                "Coordenadas inválidas. Latitude deve estar entre -90 e 90, e Longitude entre -180 e 180.");

        var resultado = await _mediator.Send(new ReverseGeocodificarQuery(latitude, longitude));
        return Ok(resultado);
    }
}