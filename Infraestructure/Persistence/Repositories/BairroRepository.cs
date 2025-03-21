using Application.Extensions.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueroPlaces.Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
///     Implementação do repositório de bairros
/// </summary>
public class BairroRepository : RepositoryBase<Bairro>, IBairroRepository
{
    public BairroRepository(AppDbContext context, ILogger<BairroRepository> logger)
        : base(context, logger)
    {
    }

    /// <summary>
    ///     Busca bairros por localidade
    /// </summary>
    public async Task<IReadOnlyList<Bairro>> GetByLocalidadeIdAsync(int localidadeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet
                .Where(b => b.LocalidadeId == localidadeId)
                .OrderBy(b => b.Nome)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar bairros pela localidade ID {LocalidadeId}", localidadeId);
            throw;
        }
    }

    /// <summary>
    ///     Busca um bairro pelo nome e localidade
    /// </summary>
    public async Task<Bairro?> GetByNomeAndLocalidadeIdAsync(string nome, int localidadeId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet
                .FirstOrDefaultAsync(b => b.Nome.ToLower() == nome.ToLower() && b.LocalidadeId == localidadeId,
                    cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar bairro pelo nome {Nome} e localidade ID {LocalidadeId}", nome,
                localidadeId);
            throw;
        }
    }

    /// <summary>
    ///     Pesquisa bairros por nome (busca parcial)
    /// </summary>
    public async Task<IReadOnlyList<Bairro>> SearchByNomeAsync(string termo, int localidadeId, int limite = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar termo de busca
            termo = termo.ToLower();

            return await DbSet
                .Where(b => b.LocalidadeId == localidadeId && b.Nome.ToLower().Contains(termo))
                .OrderBy(b => b.Nome)
                .Take(limite)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao pesquisar bairros pelo termo {Termo} e localidade ID {LocalidadeId}", termo,
                localidadeId);
            throw;
        }
    }

    /// <summary>
    ///     Sobrescreve o método GetByIdAsync para incluir relacionamentos
    /// </summary>
    public override async Task<Bairro?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet
                .Include(b => b.Localidade)
                .Include(b => b.Variacoes)
                .Include(b => b.FaixasBairro)
                .FirstOrDefaultAsync(b => b.Id == (int)id, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar bairro pelo ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    ///     Pesquisa bairros por nome em qualquer localidade
    /// </summary>
    public async Task<IReadOnlyList<Bairro>> SearchByNomeEmQualquerLocalidadeAsync(string termo, int limite = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar termo de busca
            termo = termo.ToLower();

            return await DbSet
                .Include(b => b.Localidade)
                .Where(b => b.Nome.ToLower().Contains(termo))
                .OrderBy(b => b.Nome)
                .Take(limite)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao pesquisar bairros pelo termo {Termo} em qualquer localidade", termo);
            throw;
        }
    }

    /// <summary>
    ///     Busca alternativa incluindo variações
    /// </summary>
    public async Task<IReadOnlyList<Bairro>> SearchWithVariationsAsync(string termo, int localidadeId, int limite = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar termo de busca
            termo = termo.ToLower();

            // Buscar direto pelo nome do bairro
            var bairrosPorNome = await DbSet
                .Where(b => b.LocalidadeId == localidadeId && b.Nome.ToLower().Contains(termo))
                .Take(limite)
                .ToListAsync(cancellationToken);

            // Se já encontrou o limite, não precisa buscar nas variações
            if (bairrosPorNome.Count >= limite)
                return bairrosPorNome;

            // Buscar nas variações
            var bairrosPorVariacao = await DbSet
                .Where(b => b.LocalidadeId == localidadeId &&
                            b.Variacoes != null &&
                            b.Variacoes.Any(v => v.Descricao.ToLower().Contains(termo)))
                .Take(limite - bairrosPorNome.Count)
                .ToListAsync(cancellationToken);

            // Combinar os resultados
            var resultado = new List<Bairro>(bairrosPorNome);
            resultado.AddRange(bairrosPorVariacao.Where(b => !resultado.Any(r => r.Id == b.Id)));

            return resultado.OrderBy(b => b.Nome).ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex,
                "Erro ao pesquisar bairros com variações pelo termo {Termo} e localidade ID {LocalidadeId}", termo,
                localidadeId);
            throw;
        }
    }
}