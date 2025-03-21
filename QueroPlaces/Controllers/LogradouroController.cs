using Application.Extensions.Common.Models;
using Application.Extensions.Features.Logradouro.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace QueroPlaces.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LogradouroController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogradouroController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Consulta um logradouro pelo ID
    /// </summary>
    /// <param name="id">ID do logradouro</param>
    /// <returns>Dados do logradouro ou mensagem de erro</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LogradouroDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarPorId([FromRoute] int id)
    {
        var resultado = await _mediator.Send(new ConsultarLogradouroPorIdQuery(id));

        return resultado.Match<IActionResult>(
            logradouro => Ok(logradouro),
            error => NotFound(error)
        );
    }

    /// <summary>
    ///     Consulta um logradouro pelo CEP
    /// </summary>
    /// <param name="cep">CEP no formato 00000000 ou 00000-000</param>
    /// <returns>Dados do logradouro ou mensagem de erro</returns>
    [HttpGet("cep/{cep}")]
    [ProducesResponseType(typeof(LogradouroDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConsultarPorCep([FromRoute] string cep)
    {
        // Remover hífen e espaços
        cep = cep.Replace("-", "").Trim();

        if (cep.Length != 8 || !cep.All(char.IsDigit)) return BadRequest("CEP deve conter 8 dígitos numéricos.");

        var resultado = await _mediator.Send(new ConsultarLogradouroPorCepQuery(cep));

        return resultado.Match<IActionResult>(
            logradouro => Ok(logradouro),
            error => NotFound(error)
        );
    }

    /// <summary>
    ///     Lista todos os logradouros de um bairro
    /// </summary>
    /// <param name="bairroId">ID do bairro</param>
    /// <returns>Lista de logradouros</returns>
    [HttpGet("bairro/{bairroId:int}")]
    [ProducesResponseType(typeof(IEnumerable<LogradouroDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorBairro([FromRoute] int bairroId)
    {
        var resultado = await _mediator.Send(new ConsultarLogradouroPorBairroQuery(bairroId));
        return Ok(resultado);
    }

    /// <summary>
    ///     Lista todos os logradouros de uma localidade
    /// </summary>
    /// <param name="localidadeId">ID da localidade</param>
    /// <returns>Lista de logradouros</returns>
    [HttpGet("localidade/{localidadeId:int}")]
    [ProducesResponseType(typeof(IEnumerable<LogradouroDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorLocalidade([FromRoute] int localidadeId)
    {
        var resultado = await _mediator.Send(new ConsultarLogradouroPorLocalidadeQuery(localidadeId));
        return Ok(resultado);
    }

    /// <summary>
    ///     Pesquisa logradouros por diferentes critérios
    /// </summary>
    /// <param name="request">Parâmetros de busca</param>
    /// <returns>Lista paginada de logradouros</returns>
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResultadoBuscaDTO<LogradouroDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarLogradouroQuery request)
    {
        var resultado = await _mediator.Send(request);
        return Ok(resultado);
    }
}