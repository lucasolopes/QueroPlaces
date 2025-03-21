using Application.Extensions.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueroPlaces.Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
///     Implementação do repositório de logradouros
/// </summary>
public class LogradouroRepository : RepositoryBase<Logradouro>, ILogradouroRepository
{
    public LogradouroRepository(AppDbContext context, ILogger<LogradouroRepository> logger)
        : base(context, logger)
    {
    }

    /// <summary>
    ///     Busca um logradouro pelo CEP
    /// </summary>
    public async Task<Logradouro?> GetByCEPAsync(string cep, CancellationToken cancellationToken = default)
    {
        try
        {
            // Limpar o CEP (remover - e espaços)
            cep = cep.Replace("-", "").Trim();

            return await DbSet
                .Include(l => l.Localidade)
                .Include(l => l.BairroInicial)
                .FirstOrDefaultAsync(l => l.CEP == cep, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar logradouro pelo CEP {CEP}", cep);
            throw;
        }
    }

    /// <summary>
    ///     Busca logradouros por localidade
    /// </summary>
    public async Task<IReadOnlyList<Logradouro>> GetByLocalidadeIdAsync(int localidadeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet
                .Include(l => l.BairroInicial)
                .Where(l => l.LocalidadeId == localidadeId)
                .OrderBy(l => l.Nome)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar logradouros pela localidade ID {LocalidadeId}", localidadeId);
            throw;
        }
    }

    /// <summary>
    ///     Busca logradouros por bairro
    /// </summary>
    public async Task<IReadOnlyList<Logradouro>> GetByBairroIdAsync(int bairroId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet
                .Include(l => l.Localidade)
                .Where(l => l.BairroInicialId == bairroId || l.BairroFinalId == bairroId)
                .OrderBy(l => l.Nome)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar logradouros pelo bairro ID {BairroId}", bairroId);
            throw;
        }
    }

    /// <summary>
    ///     Pesquisa logradouros por nome (busca parcial)
    /// </summary>
    public async Task<IReadOnlyList<Logradouro>> SearchByNomeAsync(string termo, int limite = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar termo de busca
            termo = termo.ToLower();

            return await DbSet
                .Include(l => l.Localidade)
                .Include(l => l.BairroInicial)
                .Where(l => l.Nome.ToLower().Contains(termo))
                .OrderBy(l => l.Nome)
                .Take(limite)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao pesquisar logradouros pelo termo {Termo}", termo);
            throw;
        }
    }

    /// <summary>
    ///     Pesquisa complexa de logradouros por múltiplos critérios
    /// </summary>
    public async Task<IReadOnlyList<Logradouro>> SearchComplexoAsync(
        string? nome,
        string? bairro,
        string? localidade,
        string? uf,
        int limite = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Construir query base
            IQueryable<Logradouro> query = DbSet
                .Include(l => l.Localidade)
                .Include(l => l.BairroInicial);

            // Aplicar filtros dinamicamente
            if (!string.IsNullOrWhiteSpace(nome))
            {
                var termoNome = nome.ToLower();
                query = query.Where(l => l.Nome.ToLower().Contains(termoNome) ||
                                         l.TipoLogradouro.ToLower().Contains(termoNome));
            }

            if (!string.IsNullOrWhiteSpace(bairro))
            {
                var termoBairro = bairro.ToLower();
                query = query.Where(
                    l => l.BairroInicial != null && l.BairroInicial.Nome.ToLower().Contains(termoBairro));
            }

            if (!string.IsNullOrWhiteSpace(localidade))
            {
                var termoLocalidade = localidade.ToLower();
                query = query.Where(l => l.Localidade != null && l.Localidade.Nome.ToLower().Contains(termoLocalidade));
            }

            if (!string.IsNullOrWhiteSpace(uf))
            {
                // Normalizar UF para maiúsculas
                uf = uf.ToUpper();
                query = query.Where(l => l.UF == uf);
            }

            // Ordenar e limitar resultados
            return await query
                .OrderBy(l => l.UF)
                .ThenBy(l => l.LocalidadeId)
                .ThenBy(l => l.Nome)
                .Take(limite)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro na pesquisa complexa de logradouros");
            throw;
        }
    }

    /// <summary>
    ///     Sobrescreve o método GetByIdAsync para incluir relacionamentos
    /// </summary>
    public override async Task<Logradouro?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet
                .Include(l => l.Localidade)
                .Include(l => l.BairroInicial)
                .Include(l => l.BairroFinal)
                .Include(l => l.Variacoes)
                .Include(l => l.Seccionamentos)
                .FirstOrDefaultAsync(l => l.Id == (int)id, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar logradouro pelo ID {Id}", id);
            throw;
        }
    }
}