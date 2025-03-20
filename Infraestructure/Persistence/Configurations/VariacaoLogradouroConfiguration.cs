using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class VariacaoLogradouroConfiguration : IEntityTypeConfiguration<VariacaoLogradouro>
{
    public void Configure(EntityTypeBuilder<VariacaoLogradouro> builder)
    {
        builder.ToTable("LOG_VAR_LOG");

        builder.HasKey(v => new { v.LogradouroId, v.VariacaoId });

        builder.Property(v => v.LogradouroId)
            .HasColumnName("LOG_NU");

        builder.Property(v => v.VariacaoId)
            .HasColumnName("VLO_NU");

        builder.Property(v => v.TipoLogradouro)
            .HasColumnName("TLO_TX")
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(v => v.Descricao)
            .HasColumnName("VLO_TX")
            .HasMaxLength(150)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(v => v.Logradouro)
            .WithMany(l => l.Variacoes)
            .HasForeignKey(v => v.LogradouroId);
    }
}