using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Entidade representando faixas de CEP de UF
/// </summary>
public class FaixaUF
{
    [Column("UFE_SG")]
    [StringLength(2)]
    public string UF { get; set; } = null!;

    [Column("UFE_CEP_INI")]
    [StringLength(8)]
    public string CEPInicial { get; set; } = null!;

    [Column("UFE_CEP_FIM")]
    [StringLength(8)]
    public string CEPFinal { get; set; } = null!;
}