namespace Application.Extensions.Common.Models;

public record EnderecoDetalhadoDTO : EnderecoDTO
{
    public int? CodigoIBGE { get; init; }
    public string? LocalidadeTipo { get; init; }
    public string? SituacaoCEP { get; init; }
    public IEnumerable<string>? VariacoesLogradouro { get; init; }
    public IEnumerable<string>? VariacoesBairro { get; init; }
    public IEnumerable<string>? VariacoesLocalidade { get; init; }
}