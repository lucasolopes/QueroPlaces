using System;

namespace Infrastructure.Elasticsearch.Configuration;

/// <summary>
/// Configurações para conexão com o Elasticsearch
/// </summary>
public class ElasticsearchSettings
{
    /// <summary>
    /// URL do servidor Elasticsearch (ex: http://elasticsearch:9200)
    /// </summary>
    public string Url { get; set; } = "http://localhost:9200";
    
    /// <summary>
    /// Nome base para os índices da aplicação
    /// </summary>
    public string IndexName { get; set; } = "addresses";
    
    /// <summary>
    /// Número de shards para os índices
    /// </summary>
    public int NumberOfShards { get; set; } = 1;
    
    /// <summary>
    /// Número de réplicas para os índices
    /// </summary>
    public int NumberOfReplicas { get; set; } = 0;
    
    /// <summary>
    /// Timeout para operações em milissegundos
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;
    
    /// <summary>
    /// Opção para habilitar HTTPS
    /// </summary>
    public bool EnableHttps { get; set; } = false;
    
    /// <summary>
    /// Opção para ignorar certificados HTTPS inválidos (útil em ambientes de desenvolvimento)
    /// </summary>
    public bool IgnoreCertificateValidation { get; set; } = true;
    
    /// <summary>
    /// Nome de usuário para autenticação básica (se necessário)
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Senha para autenticação básica (se necessário)
    /// </summary>
    public string? Password { get; set; }
    
    /// <summary>
    /// Prefixo para aliases de índice
    /// </summary>
    public string AliasPrefix => $"{IndexName}-alias";
    
    /// <summary>
    /// Intervalo para resincronização de índices (em horas)
    /// </summary>
    public int ResyncIntervalHours { get; set; } = 24;
}