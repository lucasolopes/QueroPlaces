using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Entidade representando faixas de CEP de bairros
/// </summary>
public class FaixaBairro
{
    [Column("BAI_NU")]
    public int BairroId { get; set; }

    [Column("FCB_CEP_INI")]
    [StringLength(8)]
    public string CEPInicial { get; set; } = null!;

    [Column("FCB_CEP_FIM")]
    [StringLength(8)]
    public string CEPFinal { get; set; } = null!;

    // Propriedades de navegação
    public virtual Bairro? Bairro { get; set; }
}