using Domain.Entities;

namespace Application.Extensions.Interfaces;

/// <summary>
///     Interface para repositório de Logradouros
/// </summary>
public interface ILogradouroRepository : IRepository<Logradouro>
{
    Task<Logradouro?> GetByCEPAsync(string cep, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Logradouro>> GetByLocalidadeIdAsync(int localidadeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Logradouro>> GetByBairroIdAsync(int bairroId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Logradouro>> SearchByNomeAsync(string termo, int limite = 10,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Logradouro>> SearchComplexoAsync(string? nome, string? bairro, string? localidade, string? uf,
        int limite = 10, CancellationToken cancellationToken = default);
}