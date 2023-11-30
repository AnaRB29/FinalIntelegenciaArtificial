using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSelect : MonoBehaviour
{
    [SerializeField] private Tilemap selectedTileMap;
    public event Action<CustomTile, Vector3Int> OnSelectTile;
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        var cellPos = selectedTileMap.WorldToCell(mouseWorldPos);
        var tile = selectedTileMap.GetTile(cellPos);
        if (tile == null) return;
        var customTile = tile as CustomTile;
        if (customTile == null) return;
        OnSelectTile?.Invoke(customTile, cellPos);
    }
}