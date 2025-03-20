using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class LogradouroConfiguration : IEntityTypeConfiguration<Logradouro>
{
    public void Configure(EntityTypeBuilder<Logradouro> builder)
    {
        builder.ToTable("LOG_LOGRADOURO");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasColumnName("LOG_NU")
            .ValueGeneratedNever();

        builder.Property(l => l.UF)
            .HasColumnName("UFE_SG")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(l => l.LocalidadeId)
            .HasColumnName("LOC_NU")
            .IsRequired();

        builder.Property(l => l.BairroInicialId)
            .HasColumnName("BAI_NU_INI");

        builder.Property(l => l.BairroFinalId)
            .HasColumnName("BAI_NU_FIM");

        builder.Property(l => l.Nome)
            .HasColumnName("LOG_NO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.Complemento)
            .HasColumnName("LOG_COMPLEMENTO")
            .HasMaxLength(100);

        builder.Property(l => l.CEP)
            .HasColumnName("CEP")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(l => l.TipoLogradouro)
            .HasColumnName("TLO_TX")
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(l => l.StatusTipoLogradouro)
            .HasColumnName("LOG_STA_TLO")
            .HasMaxLength(1);

        builder.Property(l => l.NomeAbreviado)
            .HasColumnName("LOG_NO_ABREV")
            .HasMaxLength(36);

        // Relacionamentos
        builder.HasOne(l => l.Localidade)
            .WithMany(loc => loc.Logradouros)
            .HasForeignKey(l => l.LocalidadeId);

        builder.HasOne(l => l.BairroInicial)
            .WithMany()
            .HasForeignKey(l => l.BairroInicialId);

        builder.HasOne(l => l.BairroFinal)
            .WithMany()
            .HasForeignKey(l => l.BairroFinalId);
    }
}