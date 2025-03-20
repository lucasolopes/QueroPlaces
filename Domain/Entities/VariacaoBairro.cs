using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Entidade representando variações de bairros
/// </summary>
public class VariacaoBairro
{
    [Column("BAI_NU")]
    public int BairroId { get; set; }

    [Column("VDB_NU")]
    public int VariacaoId { get; set; }

    [Column("VDB_TX")]
    [StringLength(72)]
    public string Descricao { get; set; } = null!;

    // Propriedades de navegação
    public virtual Bairro? Bairro { get; set; }
}