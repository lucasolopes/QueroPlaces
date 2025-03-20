using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Entidade representando faixas de CEP de localidades
/// </summary>
public class FaixaLocalidade
{
    [Column("LOC_NU")]
    public int LocalidadeId { get; set; }

    [Column("LOC_CEP_INI")]
    [StringLength(8)]
    public string CEPInicial { get; set; } = null!;

    [Column("LOC_CEP_FIM")]
    [StringLength(8)]
    public string CEPFinal { get; set; } = null!;

    [Column("LOC_TIPO_FAIXA")]
    [StringLength(1)]
    public string TipoFaixa { get; set; } = null!;

    // Propriedades de navegação
    public virtual Localidade? Localidade { get; set; }
}