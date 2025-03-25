using System;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Elasticsearch.Configuration;

/// <summary>
/// Factory para criar e configurar clientes do Elasticsearch
/// </summary>
public class ElasticsearchClientFactory
{
    private readonly ElasticsearchSettings _settings;
    private readonly ILogger<ElasticsearchClientFactory> _logger;
    private ElasticsearchClient? _client;

    public ElasticsearchClientFactory(
        IOptions<ElasticsearchSettings> options,
        ILogger<ElasticsearchClientFactory> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Cria ou retorna um cliente Elasticsearch configurado
    /// </summary>
    public ElasticsearchClient CreateClient()
    {
        if (_client != null)
            return _client;

        _logger.LogInformation("Inicializando cliente Elasticsearch com URL: {Url}", _settings.Url);

        var settings = new ElasticsearchClientSettings(new Uri(_settings.Url))
            .DefaultIndex(_settings.IndexName)
            .RequestTimeout(TimeSpan.FromMilliseconds(_settings.TimeoutMs))
            .DisableDirectStreaming();

        // Configurar autenticação se fornecida
        if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
        {
            _logger.LogInformation("Configurando autenticação básica para Elasticsearch");
            settings = settings.Authentication(new BasicAuthentication(_settings.Username, _settings.Password));
        }

        // Ignorar validação de certificado se configurado
        if (_settings.IgnoreCertificateValidation)
        {
            _logger.LogInformation("Configurando para ignorar validação de certificado HTTPS");
            settings = settings.ServerCertificateValidationCallback((_, _, _, _) => true);
        }

        try
        {
            _client = new ElasticsearchClient(settings);
            _logger.LogInformation("Cliente Elasticsearch inicializado com sucesso");
            return _client;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar cliente Elasticsearch");
            throw;
        }
    }
}