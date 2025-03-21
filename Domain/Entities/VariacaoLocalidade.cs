using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
///     Entidade representando variações de localidades
/// </summary>
public class VariacaoLocalidade
{
    [Column("LOC_NU")] public int LocalidadeId { get; set; }

    [Column("VAL_NU")] public int VariacaoId { get; set; }

    [Column("VAL_TX")] [StringLength(72)] public string Descricao { get; set; } = null!;

    // Propriedades de navegação
    public virtual Localidade? Localidade { get; set; }
}