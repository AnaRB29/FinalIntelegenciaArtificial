using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileSelect), typeof(AreaSelector))]
public class MovementPlayer : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Tilemap playerTilemap;
    [SerializeField] private TileSelect tileSelector;
    [SerializeField] private AreaSelector areaSelector;
    [SerializeField] private float speed;
    private bool _moving = false;

    private IEnumerator MoveToPosition(List<Vector3Int> cells)
    {
        _moving = true;
        cells.Reverse();
        foreach (var cell in cells)
        {
            var targetCell = playerTilemap.CellToWorld(cell);
            yield return new WaitUntil(() =>
            {
                transform.position = Vector3.MoveTowards(transform.position, targetCell, Time.deltaTime * speed);
                return (transform.position - targetCell).sqrMagnitude < 0.0005f || transform.position == targetCell;
            });
            transform.position = targetCell;
        }
        _moving = false;
    }

    private void OnSelectTile(CustomTile tile, Vector3Int position)
    {
        if (_moving) return;
        if (!areaSelector.IsAreaActive) return;
        StartCoroutine(MoveToPosition(areaSelector.SelectedPath));
        areaSelector.DeactivateArea();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_moving) return;
        var cell = playerTilemap.WorldToCell(transform.position);
        areaSelector.ActivateArea(cell, 7);
    }

    private void OnEnable()
    {
        tileSelector.OnSelectTile += OnSelectTile;
    }

    private void OnDisable()
    {
        tileSelector.OnSelectTile -= OnSelectTile;
    }
}
