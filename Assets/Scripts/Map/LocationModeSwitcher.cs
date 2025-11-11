using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.Location;
using Mapbox.Utils;
using System.Collections;

//Encargado de setear la posición inicial y cargar el mapa
public class LocationModeSwitcher : MonoBehaviour
{
    [SerializeField] private AbstractMap _map;
    public bool followLocation = true;
    private ILocationProvider _locationProvider;
    private bool _mapReady = false;
    public Vector2d currentLatLon = new Vector2d(-12.0464, -77.0428);

    IEnumerator Start()
    {
        if (_map == null)
        {
            _map = FindFirstObjectByType<AbstractMap>();
            if (_map == null)
            {
                Debug.LogError("[LocationModeSwitcher] No se encontró AbstractMap.");
                yield break;
            }
        }

        // Espera a que el mapa esté listo
        yield return new WaitUntil(() => _map.MapVisualizer?.State == ModuleState.Finished);

        _mapReady = true;
        _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
        if (_locationProvider != null)
            _locationProvider.OnLocationUpdated += LocationUpdated;

        // Setea posición inicial
        ResetInitPos();
        Debug.Log($"[LocationModeSwitcher] Mapa inicializado en {currentLatLon}");
    }

    private void LocationUpdated(Location location)
    {
#if UNITY_EDITOR
        return; // Ignorar actualizaciones en el editor
#endif
        
        if (!_mapReady || !followLocation) return;

        currentLatLon = location.LatitudeLongitude;
        _map.UpdateMap(currentLatLon);
    }
    public void ResetInitPos()
    {
        _map.UpdateMap(currentLatLon);
    }
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
