using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class FaixaLocalidadeConfiguration : IEntityTypeConfiguration<FaixaLocalidade>
{
    public void Configure(EntityTypeBuilder<FaixaLocalidade> builder)
    {
        builder.ToTable("LOG_FAIXA_LOCALIDADE");

        builder.HasKey(f => new { f.LocalidadeId, f.CEPInicial, f.TipoFaixa });

        builder.Property(f => f.LocalidadeId)
            .HasColumnName("LOC_NU");

        builder.Property(f => f.CEPInicial)
            .HasColumnName("LOC_CEP_INI")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(f => f.CEPFinal)
            .HasColumnName("LOC_CEP_FIM")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(f => f.TipoFaixa)
            .HasColumnName("LOC_TIPO_FAIXA")
            .HasMaxLength(1)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(f => f.Localidade)
            .WithMany(l => l.FaixasCEP)
            .HasForeignKey(f => f.LocalidadeId);
    }
}