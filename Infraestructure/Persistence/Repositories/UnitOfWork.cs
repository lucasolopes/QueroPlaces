using Application.Extensions.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using QueroPlaces.Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
///     Implementação do padrão UnitOfWork para gerenciar transações e repositórios
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private IBairroRepository? _bairroRepository;
    private ICEPRepository? _cepRepository;

    private ILocalidadeRepository? _localidadeRepository;
    private ILogradouroRepository? _logradouroRepository;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        AppDbContext context,
        ILoggerFactory loggerFactory)
    {
        _context = context;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<UnitOfWork>();
    }

    public ILocalidadeRepository Localidades =>
        _localidadeRepository ??=
            new LocalidadeRepository(_context, _loggerFactory.CreateLogger<LocalidadeRepository>());

    public ILogradouroRepository Logradouros =>
        _logradouroRepository ??=
            new LogradouroRepository(_context, _loggerFactory.CreateLogger<LogradouroRepository>());

    public IBairroRepository Bairros =>
        _bairroRepository ??= new BairroRepository(_context, _loggerFactory.CreateLogger<BairroRepository>());

    public ICEPRepository CEPs =>
        _cepRepository ??= new CEPRepository(_context, _loggerFactory.CreateLogger<CEPRepository>());

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar alterações no banco de dados");
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            _logger.LogWarning("Tentativa de iniciar uma transação quando já existe uma em andamento");
            return;
        }

        try
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation("Transação iniciada com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao iniciar transação");
            throw;
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("Tentativa de confirmar uma transação quando não existe nenhuma em andamento");
            return;
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Transação confirmada com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao confirmar transação");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            _logger.LogWarning("Tentativa de reverter uma transação quando não existe nenhuma em andamento");
            return;
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogInformation("Transação revertida com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao reverter transação");
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}