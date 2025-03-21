using Domain.Entities;

namespace Application.Extensions.Interfaces;

/// <summary>
///     Interface para repositório de Localidades
/// </summary>
public interface ILocalidadeRepository : IRepository<Localidade>
{
    Task<Localidade?> GetByNomeAndUFAsync(string nome, string uf, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Localidade>> GetByUFAsync(string uf, CancellationToken cancellationToken = default);
    Task<Localidade?> GetByCEPAsync(string cep, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Localidade>> SearchByNomeAsync(string termo, int limite = 10,
        CancellationToken cancellationToken = default);
}