namespace Application.Extensions.Common.Models;

public record EnderecoDTO
{
    public string? CEP { get; init; }
    public string? TipoLogradouro { get; init; }
    public string? Logradouro { get; init; }
    public string? Complemento { get; init; }
    public string? Bairro { get; init; }
    public string? Localidade { get; init; }
    public string? UF { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public string? TipoCEP { get; init; }
}