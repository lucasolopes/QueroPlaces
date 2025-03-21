using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Domain.Entities;

/// <summary>
///     Entidade representando logradouros
/// </summary>
public class Logradouro
{
    [Key] [Column("LOG_NU")] public int Id { get; set; }

    [Column("UFE_SG")] [StringLength(2)] public string UF { get; set; } = null!;

    [Column("LOC_NU")] public int LocalidadeId { get; set; }

    [Column("BAI_NU_INI")] public int? BairroInicialId { get; set; }

    [Column("BAI_NU_FIM")] public int? BairroFinalId { get; set; }

    [Column("LOG_NO")] [StringLength(100)] public string Nome { get; set; } = null!;

    [Column("LOG_COMPLEMENTO")]
    [StringLength(100)]
    public string? Complemento { get; set; }

    [Column("CEP")] [StringLength(8)] public string CEP { get; set; } = null!;

    [Column("TLO_TX")] [StringLength(36)] public string TipoLogradouro { get; set; } = null!;

    [Column("LOG_STA_TLO")]
    [StringLength(1)]
    public string? StatusTipoLogradouro { get; set; }

    [Column("LOG_NO_ABREV")]
    [StringLength(36)]
    public string? NomeAbreviado { get; set; }

    // Propriedade para coordenadas geográficas (não está no SQL, mas será útil)
    [NotMapped] public LineString? Traçado { get; set; }

    // Propriedades de navegação
    public virtual Localidade? Localidade { get; set; }
    public virtual Bairro? BairroInicial { get; set; }
    public virtual Bairro? BairroFinal { get; set; }
    public virtual ICollection<NumeroSeccionamento>? Seccionamentos { get; set; }
    public virtual ICollection<VariacaoLogradouro>? Variacoes { get; set; }
    public virtual ICollection<GrandeUsuario>? GrandesUsuarios { get; set; }
    public virtual ICollection<UnidadeOperacional>? UnidadesOperacionais { get; set; }
}