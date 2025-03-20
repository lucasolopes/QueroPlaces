using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Entidade representando faixas de CPC
/// </summary>
public class FaixaCPC
{
    [Column("CPC_NU")]
    public int CPCId { get; set; }

    [Column("CPC_INICIAL")]
    [StringLength(6)]
    public string CPCInicial { get; set; } = null!;

    [Column("CPC_FINAL")]
    [StringLength(6)]
    public string CPCFinal { get; set; } = null!;

    // Propriedades de navegação
    public virtual CaixaPostalComunitaria? CaixaPostal { get; set; }
}