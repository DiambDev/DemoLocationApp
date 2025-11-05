using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Directions;
using Mapbox.Utils;
using System.Collections.Generic;
using Mapbox.Unity;
public class RouteController : MonoBehaviour
{
    [SerializeField] private AbstractMap map;
    [SerializeField] private LineRenderer routeLine;

    private Directions directions;
    private Vector2d origin;
    private Vector2d destination;

    private void Awake()
    {
        directions = MapboxAccess.Instance.Directions;

        if (routeLine == null)
        {
            routeLine = gameObject.AddComponent<LineRenderer>();
            routeLine.positionCount = 0;
            routeLine.widthMultiplier = 0.2f;
            routeLine.material = new Material(Shader.Find("Sprites/Default"));
            routeLine.startColor = Color.green;
            routeLine.endColor = Color.red;
        }
    }

    public void SetOrigin()
    {
        origin = map.CenterLatitudeLongitude;
        Debug.Log($"📍 Origen establecido en {origin}");
    }

    public void SetDestination()
    {
        destination = map.CenterLatitudeLongitude;
        Debug.Log($"🎯 Destino establecido en {destination}");
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
        var line = new List<Vector2d>(geometry);
        routeLine.positionCount = line.Count;

        for (int i = 0; i < line.Count; i++)
        {
            var worldPos = map.GeoToWorldPosition(line[i], true);
            routeLine.SetPosition(i, worldPos);
        }

        Debug.Log("✅ Ruta dibujada correctamente.");
    }
}
