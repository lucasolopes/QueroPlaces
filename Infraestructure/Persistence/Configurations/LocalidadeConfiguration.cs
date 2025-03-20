using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class LocalidadeConfiguration : IEntityTypeConfiguration<Localidade>
{
    public void Configure(EntityTypeBuilder<Localidade> builder)
    {
        builder.ToTable("LOG_LOCALIDADE");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("LOC_NU")
            .ValueGeneratedNever();

        builder.Property(l => l.UF)
            .HasColumnName("UFE_SG")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(l => l.Nome)
            .HasColumnName("LOC_NO")
            .HasMaxLength(72)
            .IsRequired();

        builder.Property(l => l.CEP)
            .HasColumnName("CEP")
            .HasMaxLength(8);

        builder.Property(l => l.Situacao)
            .HasColumnName("LOC_IN_SIT")
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(l => l.TipoLocalidade)
            .HasColumnName("LOC_IN_TIPO_LOC")
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(l => l.LocalidadeSubordinadaId)
            .HasColumnName("LOC_NU_SUB");

        builder.Property(l => l.NomeAbreviado)
            .HasColumnName("LOC_NO_ABREV")
            .HasMaxLength(36);

        builder.Property(l => l.CodigoIBGE)
            .HasColumnName("MUN_NU");
    }
}