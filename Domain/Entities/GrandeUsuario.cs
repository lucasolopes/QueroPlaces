using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
///     Entidade representando grandes usuários (empresas, órgãos públicos, etc.)
/// </summary>
public class GrandeUsuario
{
    [Key] [Column("GRU_NU")] public int Id { get; set; }

    [Column("UFE_SG")] [StringLength(2)] public string UF { get; set; } = null!;

    [Column("LOC_NU")] public int LocalidadeId { get; set; }

    [Column("BAI_NU")] public int BairroId { get; set; }

    [Column("LOG_NU")] public int? LogradouroId { get; set; }

    [Column("GRU_NO")] [StringLength(72)] public string Nome { get; set; } = null!;

    [Column("GRU_ENDERECO")]
    [StringLength(100)]
    public string Endereco { get; set; } = null!;

    [Column("CEP")] [StringLength(8)] public string CEP { get; set; } = null!;

    [Column("GRU_NO_ABREV")]
    [StringLength(36)]
    public string? NomeAbreviado { get; set; }

    // Propriedades de navegação
    public virtual Localidade? Localidade { get; set; }
    public virtual Bairro? Bairro { get; set; }
    public virtual Logradouro? Logradouro { get; set; }
}