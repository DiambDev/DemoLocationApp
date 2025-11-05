using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities; // para conversiones/extent (según versión)

public class BlockSelector : MonoBehaviour
{
    public AbstractMap map;
    public int gridCols = 10;
    public int gridRows = 10;
    public double latMin = -12.0500, lonMin = -77.0600, latMax = -12.0300, lonMax = -77.0200;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 worldPos = hit.point;
                // Convertir world -> geo (usa el método que tu versión tenga)
                Vector2d geo = map.WorldToGeoPosition(worldPos); // si tu API no tiene, usa Conversions.WorldToGeoPosition
                OnMapTapped(geo);
            }
        }
    }

    void OnMapTapped(Vector2d latlon)
    {
        double lat = latlon.x;
        double lon = latlon.y;

        // calcula índice en la rejilla
        int col = (int)(((lon - lonMin) / (lonMax - lonMin)) * gridCols);
        int row = (int)(((lat - latMin) / (latMax - latMin)) * gridRows);

        col = Mathf.Clamp(col, 0, gridCols - 1);
        row = Mathf.Clamp(row, 0, gridRows - 1);

        Debug.Log($"Tap en geo {lat},{lon} -> cuadricula fila {row} col {col}");
        // Aquí puedes marcar la cuadricula como "peligrosa" y guardar en CSV o backend
    }
}
