using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicGraphGenerator2D : MonoBehaviour
{
    public LayerMask roadMask;       // Layer for the road
    public Tilemap roadTiles;
    public float scanRadius = 2f;   // Scan radius around the agent
    public float nodeSpacing = 2f;   // Distance between nodes
    public float edgeConnectionDistance = 3f;  // Max distance to connect nodes

    private List<Vector2> graphNodes = new List<Vector2>();
    private List<(Vector2, Vector2)> graphEdges = new List<(Vector2, Vector2)>();

    void Start()
    {
        GenerateGraph();
        ConnectNodes();
    }

    // Generate nodes dynamically along the track
    void GenerateGraph()
    {
        graphNodes.Clear();  // Clear existing nodes

        if (roadTiles == null) {
            roadTiles = GameObject.FindGameObjectWithTag("TrackTilemap").GetComponent<Tilemap>();
        }
        BoundsInt bounds = roadTiles.cellBounds;


        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);

                if (roadTiles.HasTile(cellPosition))
                {
                    Vector3 point = roadTiles.GetCellCenterWorld(cellPosition);
                    AddNode(point);
                }

            }
        }

        Debug.Log($"Generated {graphNodes.Count} nodes.");
    }


    void AddNode(Vector2 point)
    {
        if (!graphNodes.Contains(point))
        {
            graphNodes.Add(point);
        }
    }

    // Connect nearby nodes with edges
    void ConnectNodes()
{
    graphEdges.Clear();  // Clear existing edges

    for (int i = 0; i < graphNodes.Count; i++)
    {
        for (int j = i + 1; j < graphNodes.Count; j++)
        {
            float distance = Vector2.Distance(graphNodes[i], graphNodes[j]);

            // Connect nodes that are close enough (avoids cross-connecting far nodes)
            if (distance <= edgeConnectionDistance)
            {
                graphEdges.Add((graphNodes[i], graphNodes[j]));
            }
        }
    }
}


    // Draw Gizmos for debugging
    void OnDrawGizmos()
    {
        GenerateGraph();
        ConnectNodes();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, scanRadius);

        Gizmos.color = Color.green;
        foreach (var node in graphNodes)
        {
            Gizmos.DrawSphere(new Vector3(node.x, node.y, 0), 0.1f);
        }

        Gizmos.color = Color.red;
        foreach (var edge in graphEdges)
        {
            Gizmos.DrawLine(new Vector3(edge.Item1.x, edge.Item1.y, 0), new Vector3(edge.Item2.x, edge.Item2.y, 0));
        }
    }
}
