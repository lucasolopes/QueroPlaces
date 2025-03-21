namespace Application.Extensions.Common.Models;

public record BairroDTO
{
    public int Id { get; init; }
    public string Nome { get; init; } = null!;
    public int LocalidadeId { get; init; }
    public string? LocalidadeNome { get; init; }
    public string UF { get; init; } = null!;
}