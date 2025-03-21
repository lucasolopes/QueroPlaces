using System.Linq.Expressions;
using Application.Extensions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueroPlaces.Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
///     Implementação base genérica para repositórios
/// </summary>
/// <typeparam name="T">Tipo da entidade</typeparam>
public abstract class RepositoryBase<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;
    protected readonly ILogger Logger;

    protected RepositoryBase(AppDbContext context, ILogger logger)
    {
        Context = context;
        DbSet = context.Set<T>();
        Logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.FindAsync(new[] { id }, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar {Entity} pelo ID {Id}", typeof(T).Name, id);
            throw;
        }
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar todos os registros de {Entity}", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.Where(predicate).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao buscar {Entity} por predicado", typeof(T).Name);
            throw;
        }
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            await DbSet.AddAsync(entity, cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao adicionar {Entity}", typeof(T).Name);
            throw;
        }
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: não há método async para Update, então retornamos Task.CompletedTask
            Context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao atualizar {Entity}", typeof(T).Name);
            throw;
        }
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: não há método async para Remove, então retornamos Task.CompletedTask
            DbSet.Remove(entity);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Erro ao deletar {Entity}", typeof(T).Name);
            throw;
        }
    }
}