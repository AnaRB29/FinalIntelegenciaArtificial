using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Misilazo : MonoBehaviour
{
    public Queue<Vector3Int> frontier = new();
    public int range;
    public Vector3Int startPoint;
    public Vector3Int objective;
    public Set reached = new Set();
    public Tilemap tilemap;
    public float delay;
    public Dictionary<Vector3Int, Vector3Int> cameFrom = new();
    public bool canstop;
    public Transform calacaChida;
    public Grid grid;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            FloodFillStartCoroutine();
        }
        startPoint = grid.WorldToCell(calacaChida.transform.position);
    }
    public void FloodFillStartCoroutine()
    {
        frontier.Enqueue(startPoint);
        cameFrom.Add(startPoint, Vector3Int.zero);
        StartCoroutine(FloodFillCoroutine());
    }
    IEnumerator FloodFillCoroutine()
    {
        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();
            List<Vector3Int> neighbours = GetNeighbours(current);
            if (current == objective && canstop) break;
            foreach (Vector3Int next in neighbours)
            {
                if (!reached.set.Contains(next) && tilemap.GetSprite(next) != null)
                {
                    if (next != startPoint && next != objective)
                    {
                        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, 1f, 0), quaternion.Euler(0, 0, 0), Vector3.one);
                        tilemap.SetTransformMatrix(next, matrix);
                    }
                    reached.Add(next);
                    if (frontier.Count > range)
                    {
                        frontier.Enqueue(next);
                    }
                    if (!cameFrom.ContainsKey(next))
                    {
                        cameFrom.Add(next, current);
                    }
                }
            }
            yield return new WaitForSeconds(delay);
        }
        Deselect();
        frontier.Enqueue(startPoint);
        cameFrom.Add(startPoint, Vector3Int.zero);
        StartCoroutine(DownPower());
    }
    IEnumerator DownPower()
    {
        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();
            List<Vector3Int> neighbours = GetNeighbours(current);
            foreach (Vector3Int next in neighbours)
            {
                if (!reached.set.Contains(next) && tilemap.GetSprite(next) != null)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, -0.32f, 0), Quaternion.Euler(0f, 0f, 90f), Vector3.one);
                    tilemap.SetTransformMatrix(next, matrix);
                    reached.Add(next);
                    if (Vector3Int.Distance(startPoint, next) < range)
                    {
                        frontier.Enqueue(next);
                    }
                    if (!cameFrom.ContainsKey(next))
                    {
                        cameFrom.Add(next, current);
                    }
                }
            }
            yield return new WaitForSeconds(delay);
        }
        Deselect();
        frontier.Enqueue(startPoint);
        cameFrom.Add(startPoint, Vector3Int.zero);
        StartCoroutine(ClearPower());
    }
    IEnumerator ClearPower()
    {
        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();
            List<Vector3Int> neighbours = GetNeighbours(current);
            foreach (Vector3Int next in neighbours)
            {
                if (!reached.set.Contains(next) && tilemap.GetSprite(next) != null)
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, -0.12f, 0), Quaternion.Euler(0f, 0f, 0f), Vector3.one);
                    tilemap.SetTransformMatrix(next, matrix);
                    reached.Add(next);
                    if (Vector3Int.Distance(startPoint, next) < range)
                    {
                        frontier.Enqueue(next);
                    }
                    if (!cameFrom.ContainsKey(next))
                    {
                        cameFrom.Add(next, current);
                    }
                }
            }
            yield return new WaitForSeconds(delay);
        }
        Deselect();
    }
    public void Deselect()
    {
        reached.set.Clear();
        frontier.Clear();
        cameFrom.Clear();
    }
    private List<Vector3Int> GetNeighbours(Vector3Int current)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>();
        neighbours.Add(current + new Vector3Int(0, 1, 0));
        neighbours.Add(current + new Vector3Int(0, -1, 0));
        neighbours.Add(current + new Vector3Int(1, 0, 0));
        neighbours.Add(current + new Vector3Int(-1, 0, 0));
        return neighbours;
    }
}