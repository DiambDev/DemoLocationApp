using UnityEngine;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class BlockSelector : MonoBehaviour
{
    [Header("Dependencias")]
    [SerializeField] private AbstractMap map;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private float blockSize = 10f;
    [SerializeField] private float clickTolerance = 0.0001f;

    private List<Block> blocks = new List<Block>();

    private void Start()
    {
        if (map == null)
            map = FindFirstObjectByType<AbstractMap>();

        map.OnUpdated += UpdateBlockPositions;
    }

    private void OnDestroy()
    {
        if (map != null)
            map.OnUpdated -= UpdateBlockPositions;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleClick(Input.mousePosition);
    }

    private void HandleClick(Vector3 screenPos)
    {
        Vector2d latLon = ScreenToLatLon(screenPos);
        if (latLon == default(Vector2d)) return;

        Block nearby = FindNearbyBlock(latLon);

        if (nearby != null)
            nearby.ToggleWalkable();
        else
            CreateBlock(latLon);
    }

    private void CreateBlock(Vector2d latLon)
    {
        Vector3 worldPos = map.GeoToWorldPosition(latLon, true);
        
        // 👇 Nota: no lo pongas como hijo del mapa ni de este script
        GameObject newBlock = Instantiate(blockPrefab, worldPos, Quaternion.identity);

        // mantener escala fija, sin heredar del mapa
        newBlock.transform.localScale = Vector3.one * blockSize;

        Block block = newBlock.GetComponent<Block>();
        if (block == null)
            block = newBlock.AddComponent<Block>();

        block.Init(latLon, true);
        blocks.Add(block);
    }

    private Block FindNearbyBlock(Vector2d latLon)
    {
        foreach (var block in blocks)
        {
            if (block == null) continue;
            double dist = Vector2d.Distance(block.LatLon, latLon);
            if (dist < clickTolerance)
                return block;
        }
        return null;
    }

    private Vector2d ScreenToLatLon(Vector3 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, map.transform.position);


        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            return map.WorldToGeoPosition(worldPos);
        }
        return default(Vector2d);
    }

    private void UpdateBlockPositions()
    {
        foreach (var block in blocks)
        {
            if (block == null) continue;

            // se mueve con el mapa, pero mantiene su tamaño
            Vector3 worldPos = map.GeoToWorldPosition(block.LatLon, true);
            block.transform.position = worldPos;
        }
    }
}
