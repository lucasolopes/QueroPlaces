using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
///     Entidade representando países
/// </summary>
public class Pais
{
    [Key]
    [Column("PAI_SG")]
    [StringLength(2)]
    public string Sigla { get; set; } = null!;

    [Column("PAI_SG_ALTERNATIVA")]
    [StringLength(3)]
    public string? SiglaAlternativa { get; set; }

    [Column("PAI_NO_PORTUGUES")]
    [StringLength(72)]
    public string NomePortugues { get; set; } = null!;

    [Column("PAI_NO_INGLES")]
    [StringLength(72)]
    public string? NomeIngles { get; set; }

    [Column("PAI_NO_FRANCES")]
    [StringLength(72)]
    public string? NomeFrances { get; set; }

    [Column("PAI_ABREVIATURA")]
    [StringLength(36)]
    public string? Abreviatura { get; set; }
}