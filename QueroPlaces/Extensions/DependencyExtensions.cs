using Npgsql;
using StackExchange.Redis;

namespace QueroPlaces.Extensions;

public static class DependencyExtensions
{
    // Método para verificar e esperar pelo PostgreSQL ficar disponível
    public static async Task WaitForDatabaseAsync(string connectionString, ILogger logger, int maxRetries = 15,
        int retryDelaySeconds = 5)
    {
        logger.LogInformation("Aguardando conexão com PostgreSQL disponível...");

        for (var retry = 0; retry < maxRetries; retry++)
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                await connection.CloseAsync();

                logger.LogInformation("Conexão com PostgreSQL estabelecida com sucesso após {Retry} tentativas!",
                    retry + 1);
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Tentativa {Retry}/{MaxRetries} falhou. Tentando novamente em {Delay} segundos...",
                    retry + 1, maxRetries, retryDelaySeconds);

                await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
            }

        throw new ApplicationException($"Não foi possível conectar ao PostgreSQL após {maxRetries} tentativas.");
    }

    // Método para verificar e esperar pelo Redis ficar disponível
    public static async Task WaitForRedisAsync(string connectionString, ILogger logger, int maxRetries = 10,
        int retryDelaySeconds = 3)
    {
        logger.LogInformation("Aguardando conexão com Redis disponível...");

        for (var retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
                if (connection.IsConnected)
                {
                    connection.Dispose();
                    logger.LogInformation("Conexão com Redis estabelecida com sucesso após {Retry} tentativas!",
                        retry + 1);
                    return;
                }

                logger.LogWarning("Tentativa {Retry}/{MaxRetries}: Redis ainda não está conectado", retry + 1,
                    maxRetries);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Tentativa {Retry}/{MaxRetries} falhou. Tentando novamente em {Delay} segundos...",
                    retry + 1, maxRetries, retryDelaySeconds);
            }

            await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
        }

        throw new ApplicationException($"Não foi possível conectar ao Redis após {maxRetries} tentativas.");
    }

    // Método para verificar todas as dependências
    public static async Task WaitForDependenciesAsync(IConfiguration configuration, ILogger logger)
    {
        var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
        var redisConnectionString = configuration.GetConnectionString("Redis");

        // Verificar PostgreSQL
        await WaitForDatabaseAsync(dbConnectionString, logger);

        // Verificar Redis (se configurado)
        if (!string.IsNullOrEmpty(redisConnectionString)) await WaitForRedisAsync(redisConnectionString, logger);

        logger.LogInformation("Todas as dependências estão prontas e conectadas!");
    }
}