namespace Application.Extensions.Interfaces;

/// <summary>
///     Interface para Unit of Work
/// </summary>
public interface IUnitOfWork : IDisposable
{
    ILocalidadeRepository Localidades { get; }
    ILogradouroRepository Logradouros { get; }
    IBairroRepository Bairros { get; }
    ICEPRepository CEPs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}