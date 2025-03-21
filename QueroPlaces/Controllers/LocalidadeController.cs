using Application.Extensions.Common.Models;
using Application.Extensions.Features.Localidade.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace QueroPlaces.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LocalidadeController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocalidadeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Consulta uma localidade pelo ID
    /// </summary>
    /// <param name="id">ID da localidade</param>
    /// <returns>Dados da localidade ou mensagem de erro</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LocalidadeDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarPorId([FromRoute] int id)
    {
        var resultado = await _mediator.Send(new ConsultarLocalidadePorIdQuery(id));

        return resultado.Match<IActionResult>(
            localidade => Ok(localidade),
            error => NotFound(error)
        );
    }

    /// <summary>
    ///     Lista todas as localidades de uma UF
    /// </summary>
    /// <param name="uf">Sigla da UF (2 caracteres)</param>
    /// <returns>Lista de localidades</returns>
    [HttpGet("uf/{uf}")]
    [ProducesResponseType(typeof(IEnumerable<LocalidadeDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarPorUF([FromRoute] string uf)
    {
        if (string.IsNullOrWhiteSpace(uf) || uf.Length != 2)
            return BadRequest("A UF deve ter exatamente 2 caracteres.");

        var resultado = await _mediator.Send(new ConsultarLocalidadePorUFQuery { UF = uf.ToUpper() });
        return Ok(resultado);
    }

    /// <summary>
    ///     Pesquisa localidades por diferentes critérios
    /// </summary>
    /// <param name="request">Parâmetros de busca</param>
    /// <returns>Lista paginada de localidades</returns>
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResultadoBuscaDTO<LocalidadeDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarLocalidadeQuery request)
    {
        var resultado = await _mediator.Send(request);
        return Ok(resultado);
    }
}