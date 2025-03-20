using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace QueroPlaces.Infrastructure.Persistence.Context;

#pragma warning disable CS1591
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public virtual DbSet<Pais> Paises { get; set; }
    public virtual DbSet<Bairro> Bairros { get; set; }
    public virtual DbSet<CaixaPostalComunitaria> CaixasPostaisComunitarias { get; set; }
    public virtual DbSet<FaixaBairro> FaixasBairro { get; set; }
    public virtual DbSet<FaixaCPC> FaixasCPC { get; set; }
    public virtual DbSet<FaixaLocalidade> FaixasLocalidade { get; set; }
    public virtual DbSet<FaixaUF> FaixasUF { get; set; }
    public virtual DbSet<FaixaUnidadeOperacional> FaixasUnidadeOperacional { get; set; }
    public virtual DbSet<GrandeUsuario> GrandesUsuarios { get; set; }
    public virtual DbSet<Localidade> Localidades { get; set; }
    public virtual DbSet<Logradouro> Logradouros { get; set; }
    public virtual DbSet<NumeroSeccionamento> Seccionamentos { get; set; }
    public virtual DbSet<UnidadeOperacional> UnidadesOperacionais { get; set; }
    public virtual DbSet<VariacaoBairro> VariacoesBairro { get; set; }
    public virtual DbSet<VariacaoLocalidade> VariacoesLocalidade { get; set; }
    public virtual DbSet<VariacaoLogradouro> VariacoesLogradouro { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplicar configurações das entidades
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
#pragma warning restore CS1591