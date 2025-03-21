using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace QueroPlaces.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Obtendo a string de conexão
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        // Configurar Hangfire com tratamento de erro melhorado
        services.AddHangfire((sp, config) =>
        {
            // Usar o logger do DI
            var logger = sp.GetRequiredService<ILogger<Program>>();
            
            try
            {
                config.UsePostgreSqlStorage(connectionString, new PostgreSqlStorageOptions
                {
                    PrepareSchemaIfNecessary = true,
                    QueuePollInterval = TimeSpan.FromSeconds(10),
                    InvisibilityTimeout = TimeSpan.FromMinutes(10),
                    DistributedLockTimeout = TimeSpan.FromMinutes(5)
                });
                
                // Configurações adicionais
                config.UseRecommendedSerializerSettings();
                
                logger.LogInformation("Hangfire configurado com sucesso com o PostgreSQL");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao configurar Hangfire com PostgreSQL. Verifique se o banco de dados está disponível.");
                throw;
            }
        });

        // Servidor do Hangfire
        services.AddHangfireServer(options => 
        {
            options.WorkerCount = Environment.ProcessorCount * 2; // 2x número de cores
            options.Queues = new[] { "default", "import", "geocoding", "critical" }; // Filas com prioridades
            options.ServerName = $"QueroPlaces-{Environment.MachineName}";
        });

        return services;
    }

    public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration)
    {
        var dashboardOptions = new DashboardOptions
        {
            // Em ambiente de desenvolvimento, permitir acesso sem autenticação
            Authorization = new[] { new AllowAllConnectionsFilter() },
            DashboardTitle = "QueroPlaces - Processamento em Segundo Plano",
            DisplayStorageConnectionString = false // Não exibir string de conexão por segurança
        };

        return app.UseHangfireDashboard("/hangfire", dashboardOptions);
    }
}

// Filtro simples que permite todas as conexões (para desenvolvimento)
// Em produção, você deve implementar uma autenticação adequada
public class AllowAllConnectionsFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // Permitir acesso a todos
        return true;
    }
}