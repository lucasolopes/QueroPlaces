using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class FaixaUnidadeOperacionalConfiguration : IEntityTypeConfiguration<FaixaUnidadeOperacional>
{
    public void Configure(EntityTypeBuilder<FaixaUnidadeOperacional> builder)
    {
        builder.ToTable("LOG_FAIXA_UOP");

        builder.HasKey(f => new { f.UnidadeOperacionalId, f.FaixaInicial });

        builder.Property(f => f.UnidadeOperacionalId)
            .HasColumnName("UOP_NU");

        builder.Property(f => f.FaixaInicial)
            .HasColumnName("FNC_INICIAL")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(f => f.FaixaFinal)
            .HasColumnName("FNC_FINAL")
            .HasMaxLength(6)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(f => f.UnidadeOperacional)
            .WithMany(u => u.Faixas)
            .HasForeignKey(f => f.UnidadeOperacionalId);
    }
}