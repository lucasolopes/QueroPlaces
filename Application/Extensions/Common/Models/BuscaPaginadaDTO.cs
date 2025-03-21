namespace Application.Extensions.Common.Models;

public record BuscaPaginadaDTO
{
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 10;
}