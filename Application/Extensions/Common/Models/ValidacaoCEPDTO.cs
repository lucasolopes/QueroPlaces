namespace Application.Extensions.Common.Models;

public record ValidacaoCEPDTO
{
    public bool Valido { get; init; }
    public string? TipoCEP { get; init; }
    public string? Mensagem { get; init; }
}