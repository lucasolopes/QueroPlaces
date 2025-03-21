namespace Application.Extensions.Common.Models;

public record BuscaLocalidadeDTO : BuscaPaginadaDTO
{
    public string? Nome { get; init; }
    public string? UF { get; init; }
    public int? CodigoIBGE { get; init; }
}