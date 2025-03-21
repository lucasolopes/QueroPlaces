using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
///     Entidade representando faixas de Unidades Operacionais
/// </summary>
public class FaixaUnidadeOperacional
{
    [Column("UOP_NU")] public int UnidadeOperacionalId { get; set; }

    [Column("FNC_INICIAL")]
    [StringLength(8)]
    public string FaixaInicial { get; set; } = null!;

    [Column("FNC_FINAL")]
    [StringLength(6)]
    public string FaixaFinal { get; set; } = null!;

    // Propriedades de navegação
    public virtual UnidadeOperacional? UnidadeOperacional { get; set; }
}