using System;
using Elastic.Clients.Elasticsearch;
using Infrastructure.Elasticsearch.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Elasticsearch.Extensions;

/// <summary>
/// Extensões para adicionar serviços do Elasticsearch ao container de DI
/// </summary>
public static class ElasticsearchServiceExtensions
{
    /// <summary>
    /// Adiciona os serviços do Elasticsearch ao container de DI
    /// </summary>
    public static IServiceCollection AddElasticsearchServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Adiciona a configuração do Elasticsearch
        services.Configure<ElasticsearchSettings>(configuration.GetSection("Elasticsearch"));

        // Registra a factory como singleton
        services.AddSingleton<ElasticsearchClientFactory>();

        // Registra o cliente Elasticsearch como singleton
        services.AddSingleton(provider => provider.GetRequiredService<ElasticsearchClientFactory>().CreateClient());

        // Registra os serviços do Elasticsearch
        services.AddSingleton<IElasticsearchIndexService, ElasticsearchIndexService>();
        services.AddScoped<IAddressSearchService, AddressSearchService>();

        return services;
    }
}