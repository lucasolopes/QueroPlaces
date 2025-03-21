using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Application.Extensions.Extensions.GeoSpatial;

/// <summary>
///     Implementação do conversor de coordenadas
/// </summary>
public class CoordinateConverter : ICoordinateConverter
{
    private readonly GeometryFactory _geometryFactory;
    private readonly WKTReader _wktReader;
    private readonly WKTWriter _wktWriter;

    public CoordinateConverter(GeometryFactory geometryFactory)
    {
        _geometryFactory = geometryFactory;
        _wktReader = new WKTReader(geometryFactory);
        _wktWriter = new WKTWriter(); // Formato padrão WKT
    }

    public Point ToPoint(double latitude, double longitude)
    {
        // IMPORTANTE: A ordem no PostGIS é longitude,latitude (X,Y)
        return _geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
    }

    public Point ToPoint(string wkt)
    {
        return (Point)_wktReader.Read(wkt);
    }

    public (double Latitude, double Longitude) FromPoint(Point point)
    {
        // IMPORTANTE: A ordem no PostGIS é longitude,latitude (X,Y)
        return (point.Y, point.X);
    }

    public string ToWkt(Point point)
    {
        return _wktWriter.Write(point);
    }

    public string ToWkt(double latitude, double longitude)
    {
        var point = ToPoint(latitude, longitude);
        return _wktWriter.Write(point);
    }

    public LineString CreateLineString(List<Point> points)
    {
        var coordinates = points.Select(p => p.Coordinate).ToArray();
        return _geometryFactory.CreateLineString(coordinates);
    }

    public Polygon CreatePolygon(List<Point> points)
    {
        // Para criar um polígono válido, o primeiro e o último ponto devem ser iguais
        if (points.Count < 3) throw new ArgumentException("São necessários pelo menos 3 pontos para criar um polígono");

        // Verificar se o primeiro e o último ponto são iguais
        if (!points[0].Equals(points[^1]))
            // Adicionar o primeiro ponto novamente para fechar o anel
            points.Add(points[0]);

        var coordinates = points.Select(p => p.Coordinate).ToArray();
        var ring = _geometryFactory.CreateLinearRing(coordinates);
        return _geometryFactory.CreatePolygon(ring);
    }

    public double CalculateDistance(Point point1, Point point2)
    {
        return point1.Distance(point2);
    }

    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var point1 = ToPoint(lat1, lon1);
        var point2 = ToPoint(lat2, lon2);
        return CalculateDistance(point1, point2);
    }
}