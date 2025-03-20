using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class PaisConfiguration : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        builder.ToTable("ECT_PAIS");

        builder.HasKey(p => p.Sigla);

        builder.Property(p => p.Sigla)
            .HasColumnName("PAI_SG")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(p => p.SiglaAlternativa)
            .HasColumnName("PAI_SG_ALTERNATIVA")
            .HasMaxLength(3);

        builder.Property(p => p.NomePortugues)
            .HasColumnName("PAI_NO_PORTUGUES")
            .HasMaxLength(72)
            .IsRequired();

        builder.Property(p => p.NomeIngles)
            .HasColumnName("PAI_NO_INGLES")
            .HasMaxLength(72);

        builder.Property(p => p.NomeFrances)
            .HasColumnName("PAI_NO_FRANCES")
            .HasMaxLength(72);

        builder.Property(p => p.Abreviatura)
            .HasColumnName("PAI_ABREVIATURA")
            .HasMaxLength(36);
    }
}