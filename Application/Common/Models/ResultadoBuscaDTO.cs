namespace Application.Extensions.Common.Models;

public record ResultadoBuscaDTO<T>
{
    public IEnumerable<T> Resultados { get; init; } = new List<T>();
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
    public int TotalPaginas => (int)Math.Ceiling((double)Total / TamanhoPagina);
}