using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class FaixaCPCConfiguration : IEntityTypeConfiguration<FaixaCPC>
{
    public void Configure(EntityTypeBuilder<FaixaCPC> builder)
    {
        builder.ToTable("LOG_FAIXA_CPC");

        builder.HasKey(f => new { f.CPCId, f.CPCInicial });

        builder.Property(f => f.CPCId)
            .HasColumnName("CPC_NU");

        builder.Property(f => f.CPCInicial)
            .HasColumnName("CPC_INICIAL")
            .HasMaxLength(6)
            .IsRequired();

        builder.Property(f => f.CPCFinal)
            .HasColumnName("CPC_FINAL")
            .HasMaxLength(6)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(f => f.CaixaPostal)
            .WithMany(c => c.Faixas)
            .HasForeignKey(f => f.CPCId);
    }
}