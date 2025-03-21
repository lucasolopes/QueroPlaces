using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace QueroPlaces.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = HttpStatusCode.InternalServerError;
        var errorTitle = "Erro interno do servidor";
        var errorMessage = "Ocorreu um erro inesperado.";
        var details = exception.Message;

        // Determinar o tipo de exceção para fornecer uma resposta mais adequada
        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                errorTitle = "Erro de validação";
                errorMessage = "Um ou mais erros de validação ocorreram.";
                details = JsonSerializer.Serialize(validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
                break;
                
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorTitle = "Recurso não encontrado";
                errorMessage = "O recurso solicitado não foi encontrado.";
                break;
                
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                errorTitle = "Operação inválida";
                break;
                
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                errorTitle = "Acesso não autorizado";
                errorMessage = "Você não tem permissão para acessar este recurso.";
                break;
                
            default:
                // Para exceções não tratadas especificamente, log detalhado mas resposta genérica
                _logger.LogError(exception, "Exceção não tratada: {ExceptionMessage}", exception.Message);
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new
        {
            title = errorTitle,
            status = (int)statusCode,
            message = errorMessage,
            details = details,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}

// Extension method para facilitar o registro do middleware
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}