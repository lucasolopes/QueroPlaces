using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class NumeroSeccionamentoConfiguration : IEntityTypeConfiguration<NumeroSeccionamento>
{
    public void Configure(EntityTypeBuilder<NumeroSeccionamento> builder)
    {
        builder.ToTable("LOG_NUM_SEC");

        builder.HasKey(n => n.LogradouroId);

        builder.Property(n => n.LogradouroId)
            .HasColumnName("LOG_NU");

        builder.Property(n => n.NumeroInicial)
            .HasColumnName("SEC_NU_INI")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(n => n.NumeroFinal)
            .HasColumnName("SEC_NU_FIM")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(n => n.Lado)
            .HasColumnName("SEC_IN_LADO")
            .HasMaxLength(1)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(n => n.Logradouro)
            .WithMany(l => l.Seccionamentos)
            .HasForeignKey(n => n.LogradouroId);
    }
}