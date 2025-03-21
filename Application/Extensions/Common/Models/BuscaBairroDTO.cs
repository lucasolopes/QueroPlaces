namespace Application.Extensions.Common.Models;

public record BuscaBairroDTO : BuscaPaginadaDTO
{
    public string? Nome { get; init; }
    public string? Localidade { get; init; }
    public string? UF { get; init; }
}