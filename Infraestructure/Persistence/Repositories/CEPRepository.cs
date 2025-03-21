using Application.Extensions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueroPlaces.Infrastructure.Persistence.Context;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
///     Implementação do repositório de CEP
/// </summary>
public class CEPRepository : ICEPRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<CEPRepository> _logger;

    public CEPRepository(AppDbContext context, ILogger<CEPRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    ///     Verifica se um CEP existe na base de dados
    /// </summary>
    public async Task<bool> ExisteCEPAsync(string cep, CancellationToken cancellationToken = default)
    {
        try
        {
            // Limpar o CEP (remover - e espaços)
            cep = cep.Replace("-", "").Trim();

            // Verificar se é um CEP de Localidade
            var existeLocalidade = await _context.Localidades
                .AnyAsync(l => l.CEP == cep, cancellationToken);

            if (existeLocalidade)
                return true;

            // Verificar se é um CEP de Logradouro
            var existeLogradouro = await _context.Logradouros
                .AnyAsync(l => l.CEP == cep, cancellationToken);

            if (existeLogradouro)
                return true;

            // Verificar se é um CEP de Grande Usuário
            var existeGrandeUsuario = await _context.GrandesUsuarios
                .AnyAsync(g => g.CEP == cep, cancellationToken);

            if (existeGrandeUsuario)
                return true;

            // Verificar se é um CEP de Unidade Operacional
            var existeUnidadeOperacional = await _context.UnidadesOperacionais
                .AnyAsync(u => u.CEP == cep, cancellationToken);

            if (existeUnidadeOperacional)
                return true;

            // Verificar se é um CEP de Caixa Postal Comunitária
            var existeCPC = await _context.CaixasPostaisComunitarias
                .AnyAsync(c => c.CEP == cep, cancellationToken);

            if (existeCPC)
                return true;

            // Verificar se está em alguma faixa de CEP
            var existeEmFaixaUF = await _context.FaixasUF
                .AnyAsync(f => f.CEPInicial.CompareTo(cep) <= 0 &&
                               f.CEPFinal.CompareTo(cep) >= 0, cancellationToken);

            if (existeEmFaixaUF)
                return true;

            var existeEmFaixaLocalidade = await _context.FaixasLocalidade
                .AnyAsync(f => f.CEPInicial.CompareTo(cep) <= 0 &&
                               f.CEPFinal.CompareTo(cep) >= 0, cancellationToken);

            if (existeEmFaixaLocalidade)
                return true;

            var existeEmFaixaBairro = await _context.FaixasBairro
                .AnyAsync(f => f.CEPInicial.CompareTo(cep) <= 0 &&
                               f.CEPFinal.CompareTo(cep) >= 0, cancellationToken);

            return existeEmFaixaBairro;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do CEP {CEP}", cep);
            throw;
        }
    }

    /// <summary>
    ///     Obtém o tipo de um CEP baseado nas regras dos Correios
    /// </summary>
    public async Task<string?> ObterTipoCEPAsync(string cep, CancellationToken cancellationToken = default)
    {
        try
        {
            // Limpar o CEP (remover - e espaços)
            cep = cep.Replace("-", "").Trim();

            // Extrair o sufixo (3 últimos dígitos)
            if (cep.Length != 8)
                return null;

            var sufixo = int.Parse(cep.Substring(5, 3));

            // Regras para classificação do CEP baseado nos sufixos
            // Conforme comunicado dos Correios, A_COMUNICADO.md
            if (sufixo >= 900 && sufixo <= 959)
                return "GRANDE_USUARIO";

            if ((sufixo >= 970 && sufixo <= 989) || sufixo == 999)
                return "UNIDADE_OPERACIONAL";

            if (sufixo >= 990 && sufixo <= 998)
                return "CAIXA_POSTAL_COMUNITARIA";

            // Verificar nos bancos de dados para casos não cobertos pelas regras de sufixo
            if (await _context.Localidades.AnyAsync(l => l.CEP == cep, cancellationToken))
                return "LOCALIDADE";

            if (await _context.Logradouros.AnyAsync(l => l.CEP == cep, cancellationToken))
                return "LOGRADOURO";

            // Para faixas, precisamos verificar se o CEP está dentro da faixa
            var faixaUF = await _context.FaixasUF
                .FirstOrDefaultAsync(f => f.CEPInicial.CompareTo(cep) <= 0 &&
                                          f.CEPFinal.CompareTo(cep) >= 0, cancellationToken);

            if (faixaUF != null)
                return "FAIXA_UF";

            var faixaLoc = await _context.FaixasLocalidade
                .FirstOrDefaultAsync(f => f.CEPInicial.CompareTo(cep) <= 0 &&
                                          f.CEPFinal.CompareTo(cep) >= 0, cancellationToken);

            if (faixaLoc != null)
                return "FAIXA_LOCALIDADE";

            var faixaBairro = await _context.FaixasBairro
                .FirstOrDefaultAsync(f => f.CEPInicial.CompareTo(cep) <= 0 &&
                                          f.CEPFinal.CompareTo(cep) >= 0, cancellationToken);

            if (faixaBairro != null)
                return "FAIXA_BAIRRO";

            return "DESCONHECIDO";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter tipo do CEP {CEP}", cep);
            throw;
        }
    }

    /// <summary>
    ///     Valida um CEP e retorna detalhes do mesmo
    /// </summary>
    public async Task<object?> ValidarCEPAsync(string cep, CancellationToken cancellationToken = default)
    {
        try
        {
            // Limpar o CEP (remover - e espaços)
            cep = cep.Replace("-", "").Trim();

            if (cep.Length != 8 || !cep.All(char.IsDigit))
                return null;

            // Verificar se é um CEP de Localidade
            var localidade = await _context.Localidades
                .FirstOrDefaultAsync(l => l.CEP == cep, cancellationToken);

            if (localidade != null)
                return new
                {
                    Tipo = "LOCALIDADE",
                    Dados = localidade
                };

            // Verificar se é um CEP de Logradouro
            var logradouro = await _context.Logradouros
                .Include(l => l.Localidade)
                .Include(l => l.BairroInicial)
                .FirstOrDefaultAsync(l => l.CEP == cep, cancellationToken);

            if (logradouro != null)
                return new
                {
                    Tipo = "LOGRADOURO",
                    Dados = logradouro
                };

            // Verificar se é um CEP de Grande Usuário
            var grandeUsuario = await _context.GrandesUsuarios
                .Include(g => g.Localidade)
                .Include(g => g.Bairro)
                .FirstOrDefaultAsync(g => g.CEP == cep, cancellationToken);

            if (grandeUsuario != null)
                return new
                {
                    Tipo = "GRANDE_USUARIO",
                    Dados = grandeUsuario
                };

            // Verificar se é um CEP de Unidade Operacional
            var unidadeOperacional = await _context.UnidadesOperacionais
                .Include(u => u.Localidade)
                .Include(u => u.Bairro)
                .FirstOrDefaultAsync(u => u.CEP == cep, cancellationToken);

            if (unidadeOperacional != null)
                return new
                {
                    Tipo = "UNIDADE_OPERACIONAL",
                    Dados = unidadeOperacional
                };

            // Verificar se é um CEP de Caixa Postal Comunitária
            var cpc = await _context.CaixasPostaisComunitarias
                .Include(c => c.Localidade)
                .FirstOrDefaultAsync(c => c.CEP == cep, cancellationToken);

            if (cpc != null)
                return new
                {
                    Tipo = "CAIXA_POSTAL_COMUNITARIA",
                    Dados = cpc
                };

            // Verificar se está em alguma faixa de CEP
            var faixaUF = await _context.FaixasUF
                .FirstOrDefaultAsync(f => f.CEPInicial.CompareTo(cep) <= 0 &&
                                          f.CEPFinal.CompareTo(cep) >= 0, cancellationToken);

            if (faixaUF != null)
                return new
                {
                    Tipo = "FAIXA_UF",
                    Dados = faixaUF
                };

            var faixaLocalidade = await _context.FaixasLocalidade
                .Include(f => f.Localidade)
                .FirstOrDefaultAsync(f => f.CEPInicial.CompareTo(cep) <= 0 &&
                                          f.CEPFinal.CompareTo(cep) >= 0, cancellationToken);

            if (faixaLocalidade != null)
                return new
                {
                    Tipo = "FAIXA_LOCALIDADE",
                    Dados = faixaLocalidade
                };

            var faixaBairro = await _context.FaixasBairro
                .Include(f => f.Bairro)
                .ThenInclude(b => b!.Localidade)
                .FirstOrDefaultAsync(f => f.CEPInicial.CompareTo(cep) <= 0 &&
                                          f.CEPFinal.CompareTo(cep) >= 0, cancellationToken);

            if (faixaBairro != null)
                return new
                {
                    Tipo = "FAIXA_BAIRRO",
                    Dados = faixaBairro
                };

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar CEP {CEP}", cep);
            throw;
        }
    }
}