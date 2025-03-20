using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class FaixaBairroConfiguration : IEntityTypeConfiguration<FaixaBairro>
{
    public void Configure(EntityTypeBuilder<FaixaBairro> builder)
    {
        builder.ToTable("LOG_FAIXA_BAIRRO");

        builder.HasKey(f => new { f.BairroId, f.CEPInicial });

        builder.Property(f => f.BairroId)
            .HasColumnName("BAI_NU");

        builder.Property(f => f.CEPInicial)
            .HasColumnName("FCB_CEP_INI")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(f => f.CEPFinal)
            .HasColumnName("FCB_CEP_FIM")
            .HasMaxLength(8)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(f => f.Bairro)
            .WithMany(b => b.FaixasBairro)
            .HasForeignKey(f => f.BairroId);
    }
}