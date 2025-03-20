using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Entidade representando bairros
/// </summary>
public class Bairro
{
    [Key]
    [Column("BAI_NU")]
    public int Id { get; set; }

    [Column("UFE_SG")]
    [StringLength(2)]
    public string UF { get; set; } = null!;

    [Column("LOC_NU")]
    public int LocalidadeId { get; set; }

    [Column("BAI_NO")]
    [StringLength(72)]
    public string Nome { get; set; } = null!;

    [Column("BAI_NO_ABREV")]
    [StringLength(36)]
    public string? NomeAbreviado { get; set; }

    // Propriedades de navegação
    public virtual Localidade? Localidade { get; set; }
    public virtual ICollection<FaixaBairro>? FaixasBairro { get; set; }
    public virtual ICollection<VariacaoBairro>? Variacoes { get; set; }
}