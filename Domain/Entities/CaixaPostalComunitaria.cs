using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
///     Entidade representando Caixas Postais Comunitárias
/// </summary>
public class CaixaPostalComunitaria
{
    [Key] [Column("CPC_NU")] public int Id { get; set; }

    [Column("UFE_SG")] [StringLength(2)] public string UF { get; set; } = null!;

    [Column("LOC_NU")] public int LocalidadeId { get; set; }

    [Column("CPC_NO")] [StringLength(72)] public string Nome { get; set; } = null!;

    [Column("CPC_ENDERECO")]
    [StringLength(100)]
    public string Endereco { get; set; } = null!;

    [Column("CEP")] [StringLength(8)] public string CEP { get; set; } = null!;

    // Propriedades de navegação
    public virtual Localidade? Localidade { get; set; }
    public virtual ICollection<FaixaCPC>? Faixas { get; set; }
}