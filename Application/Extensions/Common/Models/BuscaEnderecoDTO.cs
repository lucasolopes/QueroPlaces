namespace Application.Extensions.Common.Models;

public record BuscaEnderecoDTO
{
    public string? CEP { get; init; }
    public string? Logradouro { get; init; }
    public string? Bairro { get; init; }
    public string? Localidade { get; init; }
    public string? UF { get; init; }
}