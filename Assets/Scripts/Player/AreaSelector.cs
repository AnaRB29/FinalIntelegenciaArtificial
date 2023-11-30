using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaSelector : MonoBehaviour
{
    [SerializeField] private Tilemap areaTilemap;

    [Header("Draw")]
    [SerializeField] private TileDrawer tileDrawer;
    [SerializeField] private CustomTile startingTileDraw;
    [SerializeField] private CustomTile areaTileDraw;
    [SerializeField] private CustomTile pathTileDraw;
    //[SerializeField] private CustomTile objectiveTileDraw;

    public List<Vector3Int> AccessibleArea { get; private set; } = new List<Vector3Int>();
    public List<Vector3Int> SelectedPath { get; private set; } = new List<Vector3Int>();

    public bool IsAreaActive => _isAreaActive;
    private bool _isAreaActive = false;

    private CustomTile startingTile;
    public Vector3Int TargetPosition => targetPosition;
    private Vector3Int startingPosition, targetPosition;

    public void ActivateArea(Vector3Int position, int tileMovement)
    {
        tileDrawer.UnDrawAll();
        startingPosition = position;

        AccessibleArea = PathFindingAlgorithm.FillLimited(startingPosition, tileMovement, areaTilemap);

        foreach (var fillTilePosition in AccessibleArea)
        {
            tileDrawer.Draw(fillTilePosition, areaTileDraw);
        }

        tileDrawer.Draw(startingPosition, startingTileDraw);

        _isAreaActive = true;
    }

    public void DeactivateArea()
    {
        tileDrawer.UnDrawAll();

        _isAreaActive = false;
    }

    private void Update()
    {
        if (!_isAreaActive) return;

        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        var newTargetPosition = areaTilemap.WorldToCell(mouseWorldPos);
        if (!AccessibleArea.Contains(newTargetPosition)) return;
        if (newTargetPosition == targetPosition) return;
        targetPosition = newTargetPosition;

        if (SelectedPath.Any())
            UnDrawSelectedPath();

        SelectedPath = PathFindingAlgorithm.PathFinding(startingPosition, targetPosition, areaTilemap);
        DrawPathTiles();
    }

    private void UnDrawSelectedPath()
    {
        foreach (var pathTile in SelectedPath)
        {
            var drawTile = AccessibleArea.Contains(pathTile) ? areaTileDraw : null;
            tileDrawer.Draw(pathTile, drawTile);
        }
    }

    private void DrawPathTiles()
    {
        foreach (var pathTile in SelectedPath)
        {
            tileDrawer.Draw(pathTile, pathTileDraw);
        }
    }
}
