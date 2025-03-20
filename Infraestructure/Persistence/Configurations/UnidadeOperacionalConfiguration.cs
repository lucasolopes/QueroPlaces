using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class UnidadeOperacionalConfiguration : IEntityTypeConfiguration<UnidadeOperacional>
{
    public void Configure(EntityTypeBuilder<UnidadeOperacional> builder)
    {
        builder.ToTable("LOG_UNID_OPER");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("UOP_NU")
            .ValueGeneratedNever();

        builder.Property(u => u.UF)
            .HasColumnName("UFE_SG")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(u => u.LocalidadeId)
            .HasColumnName("LOC_NU")
            .IsRequired();

        builder.Property(u => u.BairroId)
            .HasColumnName("BAI_NU")
            .IsRequired();

        builder.Property(u => u.LogradouroId)
            .HasColumnName("LOG_NU");

        builder.Property(u => u.Nome)
            .HasColumnName("UOP_NO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Endereco)
            .HasColumnName("UOP_ENDERECO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.CEP)
            .HasColumnName("CEP")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(u => u.IndicadorCP)
            .HasColumnName("UOP_IN_CP")
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(u => u.NomeAbreviado)
            .HasColumnName("UOP_NO_ABREV")
            .HasMaxLength(36);

        // Relacionamentos
        builder.HasOne(u => u.Localidade)
            .WithMany(l => l.UnidadesOperacionais)
            .HasForeignKey(u => u.LocalidadeId);

        builder.HasOne(u => u.Bairro)
            .WithMany()
            .HasForeignKey(u => u.BairroId);

        builder.HasOne(u => u.Logradouro)
            .WithMany(l => l.UnidadesOperacionais)
            .HasForeignKey(u => u.LogradouroId);
    }
}