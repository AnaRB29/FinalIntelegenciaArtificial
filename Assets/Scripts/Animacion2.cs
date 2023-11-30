using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Animacion2 : MonoBehaviour
{
    public Queue<Vector3Int> frontier = new();
    public int range;
    public Vector3Int startPoint;
    public Set reached = new Set();
    public Vector3Int objective;   
    public Tilemap tilemap;
    public float retraso;
    public Transform calacaChida;
    public Grid grid;

    public Dictionary<Vector3Int, Vector3Int> cameFrom = new();
    public bool canstop;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
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
            Debug.Log(frontier.Count);
            List<Vector3Int> neighbours = GetNeighbours(current);
            if (current == objective && canstop) break;
            foreach (Vector3Int next in neighbours)
            {
                if (!reached.set.Contains(next) && tilemap.GetSprite(next) != null)
                {
                    if (next != startPoint && next != objective)
                    {
                        //estas 2 líneas sirven para las animaciones, son Traslación, Rotación y Escala
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
            yield return new WaitForSeconds(retraso);
        }
        Deselect();
        frontier.Enqueue(startPoint);
        cameFrom.Add(startPoint, Vector3Int.zero);
        StartCoroutine(DownPower());
    }

    IEnumerator DownPower()
    {
        Debug.Log("Clear");

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();
            List<Vector3Int> neighbours = GetNeighbours(current);
            foreach (Vector3Int next in neighbours)
            {
                if (!reached.set.Contains(next) && tilemap.GetSprite(next) != null)
                {
                    for (int i = 0; i <= 360; i+=5)
                    {
                        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, -0.12f, 0), Quaternion.Euler(0f, 0f, 0 + i), Vector3.one);
                        tilemap.SetTransformMatrix(next, matrix);

                        yield return null;
                    }
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
            yield return new WaitForSeconds(retraso);
        }
        Deselect();
        frontier.Enqueue(startPoint);
        cameFrom.Add(startPoint, Vector3Int.zero);
        StartCoroutine(ClearPower());

    }
    IEnumerator ClearPower()
    {
        Debug.Log("Clear");

        while (frontier.Count > 0)
        {
            Vector3Int current = frontier.Dequeue();
            List<Vector3Int> neighbours = GetNeighbours(current);
            foreach (Vector3Int next in neighbours)
            {

                if (!reached.set.Contains(next) && tilemap.GetSprite(next) != null)
                {
                    for (int i = 360; i <= 0; i -= 5)
                    {
                        Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0, -0.12f, 0), Quaternion.Euler(0f, 0f, 0 + i), Vector3.one);
                        tilemap.SetTransformMatrix(next, matrix);

                        yield return null;
                    }
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
            yield return new WaitForSeconds(retraso );
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
