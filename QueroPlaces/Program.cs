using System.Reflection;
using System.Text.Json.Serialization;
using Application.Extensions;
using FluentValidation.AspNetCore;
using Hangfire;
using HealthChecks.UI.Client;
using Infraestructure.Persistence.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using QueroPlaces.Extensions;
using QueroPlaces.Middleware;
using Serilog;

// Health Checks
// Extensões personalizadas

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/quero-places-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Adicionar serviços ao contêiner
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configurar o Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "QueroPlaces API",
        Version = "v1",
        Description = "API para validação, busca e geocodificação de endereços brasileiros"
    });

    // Incluir os XMLs de documentação
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Adicionar serviços de persistência (EntityFramework com PostgreSQL)
builder.Services.AddPersistenceServices(builder.Configuration);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins("http://localhost:3000", "https://queroplaces.com.br")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Adicionar Hangfire para processamento em segundo plano
builder.Services.AddHangfireServices(builder.Configuration);

// Adicionar serviços geoespaciais (NetTopologySuite)
builder.Services.AddGeoSpatialServices();

// Adicionar cache com Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "QueroPlaces:";
});

// Adicionar FluentValidation
builder.Services.AddFluentValidationAutoValidation();

// Adicionar repositórios
builder.Services.AddRepositories();

// Adicionar compressão de resposta
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});

// Configurar Health Checks
var dbConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHealthChecks()
    .AddNpgSql(dbConnection, name: "database", tags: new[] { "db", "sql", "postgresql" });

// Adicionar Redis Health Check se estiver configurado
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnection))
    builder.Services.AddHealthChecks()
        .AddRedis(redisConnection, "redis", tags: new[] { "cache", "redis" });

// Adicionar Elasticsearch Health Check se estiver configurado
var elasticsearchUrl = builder.Configuration.GetSection("Elasticsearch:Url").Value;
if (!string.IsNullOrEmpty(elasticsearchUrl))
    builder.Services.AddHealthChecks()
        .AddElasticsearch(elasticsearchUrl, "elasticsearch", tags: new[] { "search", "elasticsearch" });

// Configurar Health Checks UI
builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(60);
    options.MaximumHistoryEntriesPerEndpoint(50);
}).AddPostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();

// Aguardar as dependências estarem prontas
if (app.Environment.IsDevelopment())
    try
    {
        // Registrar logger para logs detalhados durante a inicialização
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        // Aguardar a disponibilidade das dependências
        await DependencyExtensions.WaitForDependenciesAsync(
            builder.Configuration,
            logger);
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Falha ao aguardar pelos serviços dependentes. Encerrando aplicação...");
        await app.StopAsync();
        return 1;
    }

// Configurar o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Adicionar middleware de tratamento de exceções personalizadas
app.UseExceptionHandling();

app.UseResponseCompression();

app.UseSerilogRequestLogging();

// Configurar CORS
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

// Mapear Health Checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-api";
});

// Configurar o Dashboard do Hangfire
try
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new AllowAnonymousAuthorizationFilter() },
        DashboardTitle = "QueroPlaces - Processamento em Segundo Plano",
        DisplayStorageConnectionString = false
    });
    Log.Information("Dashboard do Hangfire configurado com sucesso");
}
catch (Exception ex)
{
    Log.Warning(ex,
        "Não foi possível inicializar o dashboard do Hangfire. Funcionalidade de processamento em segundo plano indisponível.");
}

// Iniciar a aplicação
try
{
    await app.RunAsync();
    return 0; // Retornar código de sucesso
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada inesperadamente");
    return 1; // Retornar código de erro
}