using Domain.Entities;

namespace Application.Extensions.Interfaces;

/// <summary>
///     Interface para repositório de Bairros
/// </summary>
public interface IBairroRepository : IRepository<Bairro>
{
    Task<IReadOnlyList<Bairro>> GetByLocalidadeIdAsync(int localidadeId, CancellationToken cancellationToken = default);

    Task<Bairro?> GetByNomeAndLocalidadeIdAsync(string nome, int localidadeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Bairro>> SearchByNomeAsync(string termo, int localidadeId, int limite = 10,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Bairro>> SearchByNomeEmQualquerLocalidadeAsync(string termo, int limite = 10, CancellationToken cancellationToken = default);
}