using Application.Extensions.Extensions.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Extensions.Common.Behaviors;
using Application.Extensions.GeoSpatial;
using FluentValidation;
using MediatR;

namespace Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

        // Registrar MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            
            // Adicionar comportamentos para pipeline do MediatR
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });

        // Registrar validadores do FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Adicionar serviços geoespaciais
        services.AddGeoSpatialServices();

        return services;
    }
}