namespace Application.Extensions.Common.Models;

public record ResultadoImportacaoDTO
{
    public string ArquivoOrigem { get; init; } = null!;
    public int TotalRegistros { get; init; }
    public int RegistrosImportados { get; init; }
    public int RegistrosFalha { get; init; }
    public DateTime DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public string Status { get; init; } = null!;
    public string? Mensagem { get; init; }
}