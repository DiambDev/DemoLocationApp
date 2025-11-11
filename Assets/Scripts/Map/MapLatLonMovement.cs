using UnityEngine;
using UnityEngine.EventSystems;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Unity.VisualScripting;

public class MapLatLonMovement : MonoBehaviour
{
    [Header("Mapbox References")]
    [SerializeField] public AbstractMap _map;
    [SerializeField] private LocationModeSwitcher locationSwitcher;

    [Header("Movement Settings")]
    [SerializeField] private float dragSpeed = 0.00008f;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 18f;

    public Vector2d currentCenter => _map.CenterLatitudeLongitude;

    private Vector2 lastTouchPos;
    private bool isDragging = false;
    private float lastPinchDist;

    private void Update()
    {
#if UNITY_EDITOR
        HandleMouse();
#else
        HandleTouch();
#endif
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isDragging = true;
            lastTouchPos = Input.mousePosition;
            locationSwitcher?.SetFollowLocation(false);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastTouchPos;
            MoveMap(delta);
            lastTouchPos = Input.mousePosition;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            ZoomMap(scroll * zoomSpeed * 2f);

        Debug.Log($"Scroll: {scroll}, Zoom actual: {_map.Zoom}");

    }

    private void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject(t.fingerId))
                {
                    // Desactivar seguimiento cuando comienza touch
                    if (locationSwitcher != null) locationSwitcher.SetFollowLocation(false);
                }
            }

            if (t.phase == TouchPhase.Moved && !EventSystem.current.IsPointerOverGameObject(t.fingerId))
            {
                MoveMap(t.deltaPosition);
            }
            if (t.phase == TouchPhase.Ended)
            {
                // opcional: mantener desactivado hasta que usen el botón "seguir"
            }

        }

        else if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            float currentDist = Vector2.Distance(t0.position, t1.position);

            if (t0.phase == TouchPhase.Moved || t1.phase == TouchPhase.Moved)
            {
                if (lastPinchDist != 0)
                {
                    float delta = currentDist - lastPinchDist;
                    ZoomMap(delta * 0.01f);
                }
            }

            lastPinchDist = currentDist;
        }
        else
        {
            lastPinchDist = 0;
        }
    }


    private void MoveMap(Vector2 delta)
    {
        Vector2d center = _map.CenterLatitudeLongitude;

        float latDegPerPixel = dragSpeed * Mathf.Pow(2, 14 - _map.Zoom);
        float lonDegPerPixel = latDegPerPixel / Mathf.Cos((float)(center.x * Mathf.Deg2Rad));

        center.x += delta.y * latDegPerPixel * -1;
        center.y += delta.x * lonDegPerPixel;

        _map.UpdateMap(center, _map.Zoom);
        //Debug.Log($"🧭 Nueva posición: {center}");
    }

    private void ZoomMap(float deltaZoom)
    {
        float newZoom = Mathf.Clamp(_map.Zoom + deltaZoom, minZoom, maxZoom);
        _map.UpdateMap(_map.CenterLatitudeLongitude, newZoom);

    }
    public Vector2d ScreenToLatLon(Vector3 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Si tu mapa tiene collider
            // Convierte posición del mundo a lat/lon según tu SDK
            return _map.WorldToGeoPosition(hit.point);
        }
        return default(Vector2d);
    }
}
