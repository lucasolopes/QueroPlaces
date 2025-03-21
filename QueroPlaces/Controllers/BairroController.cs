using Application.Extensions.Common.Models;
using Application.Extensions.Features.Bairro.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace QueroPlaces.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BairroController : ControllerBase
{
    private readonly IMediator _mediator;

    public BairroController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Consulta um bairro pelo ID
    /// </summary>
    /// <param name="id">ID do bairro</param>
    /// <returns>Dados do bairro ou mensagem de erro</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BairroDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarPorId([FromRoute] int id)
    {
        var resultado = await _mediator.Send(new ConsultarBairroPorIdQuery(id));

        return resultado.Match<IActionResult>(
            bairro => Ok(bairro),
            error => NotFound(error)
        );
    }

    /// <summary>
    ///     Lista todos os bairros de uma localidade
    /// </summary>
    /// <param name="localidadeId">ID da localidade</param>
    /// <returns>Lista de bairros</returns>
    [HttpGet("localidade/{localidadeId:int}")]
    [ProducesResponseType(typeof(IEnumerable<BairroDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorLocalidade([FromRoute] int localidadeId)
    {
        var resultado = await _mediator.Send(new ConsultarBairroPorLocalidadeQuery(localidadeId));
        return Ok(resultado);
    }

    /// <summary>
    ///     Pesquisa bairros por diferentes critérios
    /// </summary>
    /// <param name="request">Parâmetros de busca</param>
    /// <returns>Lista paginada de bairros</returns>
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResultadoBuscaDTO<BairroDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarBairroQuery request)
    {
        var resultado = await _mediator.Send(request);
        return Ok(resultado);
    }
}