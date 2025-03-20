using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Entidade representando variações de logradouros
/// </summary>
public class VariacaoLogradouro
{
    [Column("LOG_NU")]
    public int LogradouroId { get; set; }

    [Column("VLO_NU")]
    public int VariacaoId { get; set; }

    [Column("TLO_TX")]
    [StringLength(36)]
    public string TipoLogradouro { get; set; } = null!;

    [Column("VLO_TX")]
    [StringLength(150)]
    public string Descricao { get; set; } = null!;

    // Propriedades de navegação
    public virtual Logradouro? Logradouro { get; set; }
}