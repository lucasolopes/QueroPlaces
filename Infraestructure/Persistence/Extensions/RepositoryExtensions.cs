using Application.Extensions.Interfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructure.Persistence.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Registrar repositórios
        services.AddScoped<ILocalidadeRepository, LocalidadeRepository>();
        services.AddScoped<ILogradouroRepository, LogradouroRepository>();
        services.AddScoped<IBairroRepository, BairroRepository>();
        services.AddScoped<ICEPRepository, CEPRepository>();

        // Registrar UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}