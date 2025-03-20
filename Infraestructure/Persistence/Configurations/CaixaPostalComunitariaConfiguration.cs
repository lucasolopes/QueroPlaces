using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class CaixaPostalComunitariaConfiguration : IEntityTypeConfiguration<CaixaPostalComunitaria>
{
    public void Configure(EntityTypeBuilder<CaixaPostalComunitaria> builder)
    {
        builder.ToTable("LOG_CPC");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("CPC_NU")
            .ValueGeneratedNever();

        builder.Property(c => c.UF)
            .HasColumnName("UFE_SG")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(c => c.LocalidadeId)
            .HasColumnName("LOC_NU")
            .IsRequired();

        builder.Property(c => c.Nome)
            .HasColumnName("CPC_NO")
            .HasMaxLength(72)
            .IsRequired();

        builder.Property(c => c.Endereco)
            .HasColumnName("CPC_ENDERECO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.CEP)
            .HasColumnName("CEP")
            .HasMaxLength(8)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(c => c.Localidade)
            .WithMany(l => l.CaixasPostaisComunitarias)
            .HasForeignKey(c => c.LocalidadeId);
    }
}