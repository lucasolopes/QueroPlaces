using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Entidade representando unidades operacionais dos Correios
/// </summary>
public class UnidadeOperacional
{
    [Key]
    [Column("UOP_NU")]
    public int Id { get; set; }

    [Column("UFE_SG")]
    [StringLength(2)]
    public string UF { get; set; } = null!;

    [Column("LOC_NU")]
    public int LocalidadeId { get; set; }

    [Column("BAI_NU")]
    public int BairroId { get; set; }

    [Column("LOG_NU")]
    public int? LogradouroId { get; set; }

    [Column("UOP_NO")]
    [StringLength(100)]
    public string Nome { get; set; } = null!;

    [Column("UOP_ENDERECO")]
    [StringLength(100)]
    public string Endereco { get; set; } = null!;

    [Column("CEP")]
    [StringLength(8)]
    public string CEP { get; set; } = null!;

    [Column("UOP_IN_CP")]
    [StringLength(1)]
    public string IndicadorCP { get; set; } = null!;

    [Column("UOP_NO_ABREV")]
    [StringLength(36)]
    public string? NomeAbreviado { get; set; }

    // Propriedades de navegação
    public virtual Localidade? Localidade { get; set; }
    public virtual Bairro? Bairro { get; set; }
    public virtual Logradouro? Logradouro { get; set; }
    public virtual ICollection<FaixaUnidadeOperacional>? Faixas { get; set; }
}