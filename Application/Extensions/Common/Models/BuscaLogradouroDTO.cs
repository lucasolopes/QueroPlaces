namespace Application.Extensions.Common.Models;

public record BuscaLogradouroDTO : BuscaPaginadaDTO
{
    public string? Nome { get; init; }
    public string? TipoLogradouro { get; init; }
    public string? Bairro { get; init; }
    public string? Localidade { get; init; }
    public string? UF { get; init; }
    public string? CEP { get; init; }
}