using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class VariacaoLocalidadeConfiguration : IEntityTypeConfiguration<VariacaoLocalidade>
{
    public void Configure(EntityTypeBuilder<VariacaoLocalidade> builder)
    {
        builder.ToTable("LOG_VAR_LOC");

        builder.HasKey(v => new { v.LocalidadeId, v.VariacaoId });

        builder.Property(v => v.LocalidadeId)
            .HasColumnName("LOC_NU");

        builder.Property(v => v.VariacaoId)
            .HasColumnName("VAL_NU");

        builder.Property(v => v.Descricao)
            .HasColumnName("VAL_TX")
            .HasMaxLength(72)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(v => v.Localidade)
            .WithMany(l => l.Variacoes)
            .HasForeignKey(v => v.LocalidadeId);
    }
}