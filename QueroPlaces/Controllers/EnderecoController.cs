using Application.Extensions.Common.Models;
using Application.Extensions.Features.Endereco.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace QueroPlaces.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnderecoController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnderecoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Consulta um endereço pelo CEP
    /// </summary>
    /// <param name="cep">CEP no formato 00000000 ou 00000-000</param>
    /// <returns>Dados do endereço ou mensagem de erro</returns>
    [HttpGet("cep/{cep}")]
    [ProducesResponseType(typeof(EnderecoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConsultarPorCep([FromRoute] string cep)
    {
        // Remover hífen e espaços
        cep = cep.Replace("-", "").Trim();

        if (cep.Length != 8 || !cep.All(char.IsDigit)) return BadRequest("CEP deve conter 8 dígitos numéricos.");

        var resultado = await _mediator.Send(new ConsultarEnderecoPorCepQuery(cep));

        return resultado.Match<IActionResult>(
            endereco => Ok(endereco),
            error => NotFound(error)
        );
    }

    /// <summary>
    ///     Valida um CEP conforme regras dos Correios
    /// </summary>
    /// <param name="cep">CEP no formato 00000000 ou 00000-000</param>
    /// <returns>Resultado da validação</returns>
    [HttpGet("cep/{cep}/validar")]
    [ProducesResponseType(typeof(ValidacaoCEPDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidarCep([FromRoute] string cep)
    {
        // Remover hífen e espaços
        cep = cep.Replace("-", "").Trim();

        if (cep.Length != 8 || !cep.All(char.IsDigit)) return BadRequest("CEP deve conter 8 dígitos numéricos.");

        var resultado = await _mediator.Send(new ValidarCepQuery(cep));
        return Ok(resultado);
    }

    /// <summary>
    ///     Pesquisa endereços por diferentes critérios
    /// </summary>
    /// <param name="request">Parâmetros de busca</param>
    /// <returns>Lista paginada de endereços</returns>
    [HttpGet("pesquisar")]
    [ProducesResponseType(typeof(ResultadoBuscaDTO<EnderecoDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarEnderecoQuery request)
    {
        var resultado = await _mediator.Send(request);
        return Ok(resultado);
    }

    /// <summary>
    ///     Autocompletar para endereços
    /// </summary>
    /// <param name="termo">Termo de busca</param>
    /// <param name="tipo">Tipo do dado a ser buscado (logradouro, bairro, localidade)</param>
    /// <param name="uf">Filtrar por UF</param>
    /// <param name="limite">Número máximo de resultados</param>
    /// <returns>Lista de sugestões</returns>
    [HttpGet("autocompletar")]
    [ProducesResponseType(typeof(IEnumerable<EnderecoDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Autocompletar(
        [FromQuery] string termo,
        [FromQuery] string? tipo = null,
        [FromQuery] string? uf = null,
        [FromQuery] int limite = 10)
    {
        if (string.IsNullOrWhiteSpace(termo) || termo.Length < 3)
            return BadRequest("O termo de busca deve ter pelo menos 3 caracteres.");

        var query = new AutocompletarEnderecoQuery
        {
            Termo = termo,
            Tipo = tipo,
            UF = uf,
            Limite = limite
        };

        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }
}