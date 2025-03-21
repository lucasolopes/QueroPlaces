namespace Application.Extensions.Interfaces;

/// <summary>
///     Interface para repositório de CEPs
/// </summary>
public interface ICEPRepository
{
    Task<object?> ValidarCEPAsync(string cep, CancellationToken cancellationToken = default);
    Task<string?> ObterTipoCEPAsync(string cep, CancellationToken cancellationToken = default);
    Task<bool> ExisteCEPAsync(string cep, CancellationToken cancellationToken = default);
}