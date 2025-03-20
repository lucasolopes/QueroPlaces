using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class VariacaoBairroConfiguration : IEntityTypeConfiguration<VariacaoBairro>
{
    public void Configure(EntityTypeBuilder<VariacaoBairro> builder)
    {
        builder.ToTable("LOG_VAR_BAI");

        builder.HasKey(v => new { v.BairroId, v.VariacaoId });

        builder.Property(v => v.BairroId)
            .HasColumnName("BAI_NU");

        builder.Property(v => v.VariacaoId)
            .HasColumnName("VDB_NU");

        builder.Property(v => v.Descricao)
            .HasColumnName("VDB_TX")
            .HasMaxLength(72)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(v => v.Bairro)
            .WithMany(b => b.Variacoes)
            .HasForeignKey(v => v.BairroId);
    }
}