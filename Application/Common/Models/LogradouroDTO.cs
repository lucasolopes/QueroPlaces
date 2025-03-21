namespace Application.Extensions.Common.Models;

public record LogradouroDTO
{
    public int Id { get; init; }
    public string TipoLogradouro { get; init; } = null!;
    public string Nome { get; init; } = null!;
    public string? Complemento { get; init; }
    public string CEP { get; init; } = null!;
    public int LocalidadeId { get; init; }
    public string? LocalidadeNome { get; init; }
    public int? BairroId { get; init; }
    public string? BairroNome { get; init; }
    public string UF { get; init; } = null!;
}