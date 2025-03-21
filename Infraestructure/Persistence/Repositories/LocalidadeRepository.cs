using Application.Extensions.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueroPlaces.Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
///     Implementação do repositório de localidades
/// </summary>
public class LocalidadeRepository : RepositoryBase<Localidade>, ILocalidadeRepository
{
    public LocalidadeRepository(AppDbContext context, ILogger<LocalidadeRepository> logger)
        : base(context, logger)
    {
    }

    /// <summary>
    ///     Busca uma localidade pelo CEP
    /// </summary>
    public async Task<Localidade?> GetByCEPAsync(string cep, CancellationToken cancellationToken = default)
    {
        try
        {
            // Limpar o CEP (remover - e espaços)
            cep = cep.Replace("-", "").Trim();

            return await DbSet
                .FirstOrDefaultAsync(l => l.CEP == cep, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar localidade pelo CEP {CEP}", cep);
            throw;
        }
    }

    /// <summary>
    ///     Busca uma localidade pelo nome e UF
    /// </summary>
    public async Task<Localidade?> GetByNomeAndUFAsync(string nome, string uf,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar UF para maiúsculas
            uf = uf.ToUpper();

            return await DbSet
                .FirstOrDefaultAsync(l => l.Nome.ToLower() == nome.ToLower() && l.UF == uf, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar localidade pelo nome {Nome} e UF {UF}", nome, uf);
            throw;
        }
    }

    /// <summary>
    ///     Busca localidades por UF
    /// </summary>
    public async Task<IReadOnlyList<Localidade>> GetByUFAsync(string uf, CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar UF para maiúsculas
            uf = uf.ToUpper();

            return await DbSet
                .Where(l => l.UF == uf)
                .OrderBy(l => l.Nome)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar localidades pela UF {UF}", uf);
            throw;
        }
    }

    /// <summary>
    ///     Pesquisa localidades por nome (busca parcial)
    /// </summary>
    public async Task<IReadOnlyList<Localidade>> SearchByNomeAsync(string termo, int limite = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar termo de busca
            termo = termo.ToLower();

            return await DbSet
                .Where(l => l.Nome.ToLower().Contains(termo))
                .OrderBy(l => l.Nome)
                .Take(limite)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao pesquisar localidades pelo termo {Termo}", termo);
            throw;
        }
    }

    /// <summary>
    ///     Sobrescreve o método GetAllAsync para incluir relacionamentos
    /// </summary>
    public override async Task<IReadOnlyList<Localidade>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet
                .OrderBy(l => l.UF)
                .ThenBy(l => l.Nome)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar todas as localidades");
            throw;
        }
    }

    /// <summary>
    ///     Sobrescreve o método GetByIdAsync para incluir relacionamentos
    /// </summary>
    public override async Task<Localidade?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet
                .Include(l => l.Bairros)
                .Include(l => l.Variacoes)
                .FirstOrDefaultAsync(l => l.Id == (int)id, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar localidade pelo ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    ///     Busca alternativa incluindo variações
    /// </summary>
    public async Task<IReadOnlyList<Localidade>> SearchWithVariationsAsync(string termo, int limite = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Normalizar termo de busca
            termo = termo.ToLower();

            // Buscar direto pelo nome da localidade
            var localidadesPorNome = await DbSet
                .Where(l => l.Nome.ToLower().Contains(termo))
                .Take(limite)
                .ToListAsync(cancellationToken);

            // Se já encontrou o limite, não precisa buscar nas variações
            if (localidadesPorNome.Count >= limite)
                return localidadesPorNome;

            // Buscar nas variações
            var localidadesPorVariacao = await DbSet
                .Where(l => l.Variacoes != null && l.Variacoes.Any(v => v.Descricao.ToLower().Contains(termo)))
                .Take(limite - localidadesPorNome.Count)
                .ToListAsync(cancellationToken);

            // Combinar os resultados
            var resultado = new List<Localidade>(localidadesPorNome);
            resultado.AddRange(localidadesPorVariacao.Where(l => !resultado.Any(r => r.Id == l.Id)));

            return resultado.OrderBy(l => l.Nome).ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao pesquisar localidades com variações pelo termo {Termo}", termo);
            throw;
        }
    }
}