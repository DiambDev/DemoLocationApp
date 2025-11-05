using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using System.Collections;

public class LocationModeSwitcher : MonoBehaviour
{
    [SerializeField] private AbstractMap _map;
    public bool followLocation = true; 
    private ILocationProvider _locationProvider;
    private bool _mapReady = false;
    private Vector2d _currentLatLon = new Vector2d(-12.0464, -77.0428); // Lima

    IEnumerator Start()
    {
        // Buscar mapa
        if (_map == null)
        {
            _map = FindFirstObjectByType<AbstractMap>();
            if (_map == null)
            {
                Debug.LogError("[LocationModeSwitcher] No se encontró ningún AbstractMap en la escena.");
                yield break;
            }
        }

        // Esperar a que MapVisualizer y TileProvider estén inicializados
        yield return new WaitUntil(() => _map.MapVisualizer != null && _map.MapVisualizer.State == ModuleState.Finished);
        yield return new WaitUntil(() => _map.TileProvider != null);

        _mapReady = true;
        Debug.Log("[LocationModeSwitcher] Mapa completamente listo.");

        // Configurar proveedor de ubicación
        _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
        if (_locationProvider != null)
        {
            _locationProvider.OnLocationUpdated += LocationUpdated;
        }
        else
        {
            Debug.LogWarning("[LocationModeSwitcher] No se encontró LocationProvider.");
        }
    }

    private void LocationUpdated(Location location)
    {
        if (!_mapReady || _map == null) return;

        // Validación básica
        if (!location.IsLocationServiceEnabled) return;

        Vector2d latLon;

#if UNITY_EDITOR
        // En editor puedes usar coordenadas de prueba solo si followLocation está activado
        latLon = new Vector2d(-12.0464, -77.0428);
#else
        latLon = location.LatitudeLongitude;
#endif

        if (!followLocation)
        {
            // Si el usuario desactivó el seguimiento, no cambiamos el centro
            return;
        }

        try
        {
            _map.UpdateMap(latLon);
            Debug.Log($"[LocationModeSwitcher] Mapa actualizado (follow): {latLon.x}, {latLon.y}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[LocationModeSwitcher] Error al actualizar mapa: {ex.Message}");
        }
    }

    // Método público para controlar follow desde otros scripts
    public void SetFollowLocation(bool follow)
    {
        followLocation = follow;
        Debug.Log($"[LocationModeSwitcher] followLocation = {follow}");
    }
    void OnDestroy()
    {
        if (_locationProvider != null)
            _locationProvider.OnLocationUpdated -= LocationUpdated;
    }

}
