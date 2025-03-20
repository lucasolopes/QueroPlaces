using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Persistence.Configurations;

public class GrandeUsuarioConfiguration : IEntityTypeConfiguration<GrandeUsuario>
{
    public void Configure(EntityTypeBuilder<GrandeUsuario> builder)
    {
        builder.ToTable("LOG_GRANDE_USUARIO");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("GRU_NU")
            .ValueGeneratedNever();

        builder.Property(g => g.UF)
            .HasColumnName("UFE_SG")
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(g => g.LocalidadeId)
            .HasColumnName("LOC_NU")
            .IsRequired();

        builder.Property(g => g.BairroId)
            .HasColumnName("BAI_NU")
            .IsRequired();

        builder.Property(g => g.LogradouroId)
            .HasColumnName("LOG_NU");

        builder.Property(g => g.Nome)
            .HasColumnName("GRU_NO")
            .HasMaxLength(72)
            .IsRequired();

        builder.Property(g => g.Endereco)
            .HasColumnName("GRU_ENDERECO")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(g => g.CEP)
            .HasColumnName("CEP")
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(g => g.NomeAbreviado)
            .HasColumnName("GRU_NO_ABREV")
            .HasMaxLength(36);

        // Relacionamentos
        builder.HasOne(g => g.Localidade)
            .WithMany(l => l.GrandesUsuarios)
            .HasForeignKey(g => g.LocalidadeId);

        builder.HasOne(g => g.Bairro)
            .WithMany()
            .HasForeignKey(g => g.BairroId);

        builder.HasOne(g => g.Logradouro)
            .WithMany(l => l.GrandesUsuarios)
            .HasForeignKey(g => g.LogradouroId);
    }
}