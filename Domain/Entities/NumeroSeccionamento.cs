using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Entidade representando seccionamentos de logradouros
/// </summary>
public class NumeroSeccionamento
{
    [Column("LOG_NU")]
    public int LogradouroId { get; set; }

    [Column("SEC_NU_INI")]
    [StringLength(10)]
    public string NumeroInicial { get; set; } = null!;

    [Column("SEC_NU_FIM")]
    [StringLength(10)]
    public string NumeroFinal { get; set; } = null!;

    [Column("SEC_IN_LADO")]
    [StringLength(1)]
    public string Lado { get; set; } = null!;

    // Propriedades de navegação
    public virtual Logradouro? Logradouro { get; set; }
}