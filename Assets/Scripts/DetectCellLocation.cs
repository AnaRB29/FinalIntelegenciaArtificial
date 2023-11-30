using UnityEngine;
using UnityEngine.Tilemaps;

public class DetectCellLocation : MonoBehaviour
{

    public GridLayout gridLayout;
    private Vector3 worldPosition;
    public Tilemap tilemap;
    public TileBase origen;
    public TileBase destino;
    public TileBase encimado;
    public TileBase original;
    public Misilazo startpoint;
    public Misilazo endpoint;
    public bool FloodFillAlg;
    public bool FloodFillEarlyExit;
    public bool DijkstrasAlg;
    public bool HeuristicAlg;
    public bool HeuristicEarlyExit;
    public bool AEstrellaAlg;
    public bool AEstrellaEarlyExit;
    private TileBase originalTileBase;
    private Vector3Int? origenTile;
    private Vector3Int? originalTile;
    private Vector3Int? destinoTile;

    private void Start()
    {
        origenTile = null;
    }
    private void Update()
    {
        if (FloodFillAlg == true)
        {
            FloodFill();
        }
        else if (FloodFillEarlyExit == true)
        {
            FloodFill();
            startpoint.canstop = true;
        }
        else if (DijkstrasAlg == true)
        {
            startpoint.enabled = false;
        }
        else if (HeuristicAlg == true)
        {
            Heuristic();
            startpoint.enabled = false;

        }
        else if (HeuristicEarlyExit == true)
        {
            Heuristic();
            startpoint.enabled = false;
        }
        else if (AEstrellaAlg == true)
        {
            startpoint.canstop = true;
        }
        else if (AEstrellaEarlyExit == true)
        {
            startpoint.canstop = true;
        }
    }
    private Vector3Int GetPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3Int cellPosition = gridLayout.WorldToCell(worldPosition);
        mousePos = gridLayout.CellToWorld(cellPosition);
        cellPosition.z = 0;
        return cellPosition;
    }
    public void FloodFill()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var actualTile = tilemap.GetTile(GetPosition());
            if (actualTile == null) { return; }
            startpoint.startPoint = GetPosition();
            if (origenTile != null) { }
            origenTile = GetPosition();
        }

        if (Input.GetMouseButtonDown(1))
        {
            var actualTile = tilemap.GetTile(GetPosition());
            if (actualTile == null) { return; }
            endpoint.objective = GetPosition();
            if (destinoTile != null) { }
            destinoTile = GetPosition();
        }

        if (originalTile != GetPosition())
        {
            if (originalTile != null && originalTileBase != null && originalTile.Value != origenTile && originalTile.Value != destinoTile) { }
            originalTile = GetPosition();
            originalTileBase = tilemap.GetTile(GetPosition());
            if (tilemap.GetSprite(GetPosition()) != null && originalTile.Value != origenTile && originalTile.Value != destinoTile){ }
        }
    }

    public void Heuristic()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var actualTile = tilemap.GetTile(GetPosition());
            if (actualTile == null) { return; }
            if (origenTile != null) { }
            origenTile = GetPosition();
        }

        if (Input.GetMouseButtonDown(1))
        {
            var actualTile = tilemap.GetTile(GetPosition());
            if (actualTile == null) { return; }
            if (destinoTile != null) { }
            destinoTile = GetPosition();
        }

        if (originalTile != GetPosition())
        {
            if (originalTile != null && originalTileBase != null && originalTile.Value != origenTile && originalTile.Value != destinoTile) { }
            originalTile = GetPosition();
            originalTileBase = tilemap.GetTile(GetPosition());

            if (tilemap.GetSprite(GetPosition()) != null && originalTile.Value != origenTile && originalTile.Value != destinoTile){ }
        }
    }


}