using Application.Extensions.Extensions.GeoSpatial;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Application.Extensions.GeoSpatial;

/// <summary>
///     Extensões para adicionar serviços geoespaciais à aplicação
/// </summary>
public static class GeoSpatialExtensions
{
    /// <summary>
    ///     Adiciona serviços geoespaciais ao contêiner de DI
    /// </summary>
    public static IServiceCollection AddGeoSpatialServices(this IServiceCollection services)
    {
        // Configurar o SRID padrão para 4326 (WGS84 - coordenadas geográficas em graus)
        // Este é o padrão usado para latitude/longitude no GPS e na maioria dos sistemas
        const int srid = 4326;

        // Configurar o NTS Geometry Factory para trabalhar com o SRID especificado
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid);
        services.AddSingleton(geometryFactory);

        // Configurar o serializador GeoJSON
        var serializer = GeoJsonSerializer.Create(new GeometryFactory(new PrecisionModel(), srid));
        services.AddSingleton(serializer);

        // Adicionar conversores de coordenadas
        services.AddSingleton<ICoordinateConverter, CoordinateConverter>();

        return services;
    }
}