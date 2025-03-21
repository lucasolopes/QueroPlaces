namespace Application.Extensions.Common.Models;

public record EnderecoDTO
{
    public string? CEP { get; set; }
    public string? TipoLogradouro { get; set; }
    public string? Logradouro { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Localidade { get; set; }
    public string? UF { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? TipoCEP { get; set; }
}