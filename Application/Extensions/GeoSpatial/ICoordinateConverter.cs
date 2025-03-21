using NetTopologySuite.Geometries;

namespace Application.Extensions.Extensions.GeoSpatial;

/// <summary>
///     Interface para conversão entre diferentes formatos de coordenadas
/// </summary>
public interface ICoordinateConverter
{
    /// <summary>
    ///     Converte latitude e longitude para um objeto Point do NTS
    /// </summary>
    Point ToPoint(double latitude, double longitude);

    /// <summary>
    ///     Converte texto WKT para um objeto Point do NTS
    /// </summary>
    Point ToPoint(string wkt);

    /// <summary>
    ///     Extrai latitude e longitude de um objeto Point do NTS
    /// </summary>
    (double Latitude, double Longitude) FromPoint(Point point);

    /// <summary>
    ///     Converte um ponto para o formato WKT
    /// </summary>
    string ToWkt(Point point);

    /// <summary>
    ///     Converte latitude e longitude para o formato WKT
    /// </summary>
    string ToWkt(double latitude, double longitude);

    /// <summary>
    ///     Cria uma linha a partir de uma lista de pontos
    /// </summary>
    LineString CreateLineString(List<Point> points);

    /// <summary>
    ///     Cria um polígono a partir de uma lista de pontos
    /// </summary>
    Polygon CreatePolygon(List<Point> points);

    /// <summary>
    ///     Calcula a distância entre dois pontos em metros
    /// </summary>
    double CalculateDistance(Point point1, Point point2);

    /// <summary>
    ///     Calcula a distância entre dois pares de coordenadas em metros
    /// </summary>
    double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
}