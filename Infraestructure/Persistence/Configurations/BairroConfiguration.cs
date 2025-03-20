using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class BairroConfiguration : IEntityTypeConfiguration<Bairro>
{
    public void Configure(EntityTypeBuilder<Bairro> builder)
    {
        builder.ToTable("LOG_BAIRRO");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("BAI_NU")
            .ValueGeneratedNever();

        builder.Property(b => b.UF)
            .HasColumnName("UFE_SG")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(b => b.LocalidadeId)
            .HasColumnName("LOC_NU")
            .IsRequired();

        builder.Property(b => b.Nome)
            .HasColumnName("BAI_NO")
            .HasMaxLength(72)
            .IsRequired();

        builder.Property(b => b.NomeAbreviado)
            .HasColumnName("BAI_NO_ABREV")
            .HasMaxLength(36);

        // Relacionamentos
        builder.HasOne(b => b.Localidade)
            .WithMany(l => l.Bairros)
            .HasForeignKey(b => b.LocalidadeId);
    }
}