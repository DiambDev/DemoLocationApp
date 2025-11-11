using UnityEngine;
using Mapbox.Utils;

public class Block : MonoBehaviour
{
    public bool IsWalkable { get; private set; } = true;
    public Vector2d LatLon { get; private set; }

    private Renderer rend;

    public void Init(Vector2d latLon, bool isWalkable)
    {
        LatLon = latLon;
        IsWalkable = isWalkable;
        rend = GetComponent<Renderer>();
        UpdateVisual();
    }

    public void ToggleWalkable()
    {
        IsWalkable = !IsWalkable;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        rend.material.color = IsWalkable ? Color.green : Color.red;
    }
}
