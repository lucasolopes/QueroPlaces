using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class FaixaUFConfiguration : IEntityTypeConfiguration<FaixaUF>
{
    public void Configure(EntityTypeBuilder<FaixaUF> builder)
    {
        builder.ToTable("LOG_FAIXA_UF");

        builder.HasKey(f => new { f.UF, f.CEPInicial });

        builder.Property(f => f.UF)
            .HasColumnName("UFE_SG")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(f => f.CEPInicial)
            .HasColumnName("UFE_CEP_INI")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(f => f.CEPFinal)
            .HasColumnName("UFE_CEP_FIM")
            .HasMaxLength(8)
            .IsRequired();
    }
}