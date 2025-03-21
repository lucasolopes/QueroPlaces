namespace Application.Extensions.Common.Models;

public record LocalidadeDTO
{
    public int Id { get; init; }
    public string Nome { get; init; } = null!;
    public string UF { get; init; } = null!;
    public string? CEP { get; init; }
    public string? TipoLocalidade { get; init; }
    public int? CodigoIBGE { get; init; }
}