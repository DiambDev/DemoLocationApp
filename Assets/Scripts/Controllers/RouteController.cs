using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Directions;
using Mapbox.Utils;
using System.Collections.Generic;
using Mapbox.Unity;
using System;
public class RouteController : MonoBehaviour
{
    [SerializeField] private AbstractMap map;
    [SerializeField] private LineRenderer routeLine;
    [SerializeField] private MapLatLonMovement mapMovement;
    [SerializeField] private GameObject startPinPrefab;
    [SerializeField] private GameObject endPinPrefab;
    [SerializeField] private Material routeMaterial;
    private List<Vector2d> routePoints;
    private GameObject startPin;
    private GameObject endPin;
    private Directions directions;
    private Vector2d origin;
    public Vector2d Origin=>origin;
    private Vector2d destination;
    public Vector2d Destination=>destination;

    private void Awake()
    {
        directions = MapboxAccess.Instance.Directions;

        if (routeLine == null)
        {
            routeLine = gameObject.AddComponent<LineRenderer>();
            routeLine.positionCount = 0;
            routeLine.widthMultiplier = 2.5f;
            routeLine.material = new Material(Shader.Find("Sprites/Default"));
            routeLine.startColor = Color.green;
            routeLine.endColor = Color.red;
        }
    }


    private void Start()
    {
        map.OnUpdated += UpdatePinsPosition;
    }
    public void SetOrigin()
    {
        origin = mapMovement.currentCenter; 
        if (startPin == null)
        startPin = Instantiate(startPinPrefab);
        startPin.transform.position = map.GeoToWorldPosition(origin, true);

        Debug.Log($"📍 Origen establecido en {origin}");
    }
    public void SetDestination()
    {
        destination = mapMovement.currentCenter;
        if (endPin == null)
        endPin = Instantiate(endPinPrefab);
        endPin.transform.position = map.GeoToWorldPosition(destination, true);
        Debug.Log($"🎯 Destino establecido en {destination}");
    }
    private void UpdatePinsPosition()
    {
        if (startPin != null)
            startPin.transform.position = map.GeoToWorldPosition(origin, true);

        if (endPin != null)
            endPin.transform.position = map.GeoToWorldPosition(destination, true);

        if (routeLine != null && routePoints != null)
        {
            for (int i = 0; i < routePoints.Count; i++)
                routeLine.SetPosition(i, map.GeoToWorldPosition(routePoints[i], true));
        }
    }

    public void DrawRoute()
    {
        if (origin.Equals(default(Vector2d)) || destination.Equals(default(Vector2d)))
        {
            Debug.LogWarning("⚠️ Debes establecer el origen y destino antes de trazar la ruta.");
            return;
        }

        var wp = new List<Vector2d> { origin, destination };
        var directionResource = new DirectionResource(wp.ToArray(), RoutingProfile.Driving)
        {
            Steps = true
        };
        //CenterMapOnRoute(routePoints);

        directions.Query(directionResource, HandleDirectionsResponse);
    }

    private void HandleDirectionsResponse(DirectionsResponse response)
    {
        if (response == null || response.Routes == null || response.Routes.Count == 0)
        {
            Debug.LogError("❌ No se pudo obtener la ruta.");
            return;
        }

        var geometry = response.Routes[0].Geometry;
        routePoints = new List<Vector2d>(geometry); // ← Guardas aquí las coordenadas

        routeLine.positionCount = routePoints.Count;

        for (int i = 0; i < routePoints.Count; i++)
            routeLine.SetPosition(i, map.GeoToWorldPosition(routePoints[i], true));

        Debug.Log("✅ Ruta dibujada correctamente.");
    }

    private void CenterMapOnRoute(List<Vector2d> route)
    {
        if (route == null || route.Count == 0) return;

        // centro por promedio
        double avgLat = 0, avgLon = 0;
        foreach (var p in route) { avgLat += p.x; avgLon += p.y; }
        avgLat /= route.Count; avgLon /= route.Count;
        Vector2d center = new Vector2d(avgLat, avgLon);

        // distancia máxima entre puntos (metros)
        double maxDist = 0;
        for (int i = 0; i < route.Count; i++)
        {
            for (int j = i + 1; j < route.Count; j++)
            {
                double d = HaversineDistance(route[i], route[j]);
                if (d > maxDist) maxDist = d;
            }
        }

        float targetZoom = GetZoomForDistance(maxDist, (float)center.x);

        // aplica centro + zoom
        map.UpdateMap(center, targetZoom);
    }
    /// <summary>Distancia Haversine en metros entre dos lat/lon (Vector2d: x=lat, y=lon)</summary>
private double HaversineDistance(Vector2d a, Vector2d b)
{
    const double R = 6371000.0; // metros
    double lat1 = a.x * Mathf.Deg2Rad;
    double lat2 = b.x * Mathf.Deg2Rad;
    double dLat = (b.x - a.x) * Mathf.Deg2Rad;
    double dLon = (b.y - a.y) * Mathf.Deg2Rad;

    double sinLat = Math.Sin(dLat / 2.0);
    double sinLon = Math.Sin(dLon / 2.0);
    double aa = sinLat * sinLat + Math.Cos(lat1) * Math.Cos(lat2) * sinLon * sinLon;
    double c = 2.0 * Math.Atan2(Math.Sqrt(aa), Math.Sqrt(1 - aa));
    return R * c;
}

/// <summary>Heurística para convertir distancia (m) a zoom (approx 3-18)</summary>
private float GetZoomForDistance(double maxDistanceMeters, float centerLat)
{
    if (maxDistanceMeters <= 0) return map.Zoom;
    if (maxDistanceMeters < 50) return 18f;
    if (maxDistanceMeters < 200) return 16f;
    if (maxDistanceMeters < 1000) return 14f;
    if (maxDistanceMeters < 5000) return 12f;
    if (maxDistanceMeters < 20000) return 10f;
    if (maxDistanceMeters < 80000) return 8f;
    return 6f;
}
    private void OnDestroy()
    {
        map.OnUpdated -= UpdatePinsPosition;
    }

}
