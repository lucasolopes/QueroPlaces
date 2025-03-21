using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Domain.Entities;

/// <summary>
///     Entidade representando localidades (municípios, distritos e povoados)
/// </summary>
public class Localidade
{
    [Key] [Column("LOC_NU")] public int Id { get; set; }

    [Column("UFE_SG")] [StringLength(2)] public string UF { get; set; } = null!;

    [Column("LOC_NO")] [StringLength(72)] public string Nome { get; set; } = null!;

    [Column("CEP")] [StringLength(8)] public string? CEP { get; set; }

    [Column("LOC_IN_SIT")]
    [StringLength(1)]
    public string Situacao { get; set; } = null!;

    [Column("LOC_IN_TIPO_LOC")]
    [StringLength(1)]
    public string TipoLocalidade { get; set; } = null!;

    [Column("LOC_NU_SUB")] public int? LocalidadeSubordinadaId { get; set; }

    [Column("LOC_NO_ABREV")]
    [StringLength(36)]
    public string? NomeAbreviado { get; set; }

    [Column("MUN_NU")] public int? CodigoIBGE { get; set; }

    // Propriedade para coordenadas geográficas (não está no SQL, mas será útil)
    [NotMapped] public Point? Coordenadas { get; set; }

    // Propriedades de navegação
    public virtual ICollection<Bairro>? Bairros { get; set; }
    public virtual ICollection<Logradouro>? Logradouros { get; set; }
    public virtual ICollection<FaixaLocalidade>? FaixasCEP { get; set; }
    public virtual ICollection<VariacaoLocalidade>? Variacoes { get; set; }
    public virtual ICollection<GrandeUsuario>? GrandesUsuarios { get; set; }
    public virtual ICollection<UnidadeOperacional>? UnidadesOperacionais { get; set; }
    public virtual ICollection<CaixaPostalComunitaria>? CaixasPostaisComunitarias { get; set; }
}