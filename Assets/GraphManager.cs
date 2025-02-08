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
    public float curveMaxRadiusToConsider = 1f;
    public float distanceToNextCurvedNode = .5f;
    public Transform viewOrigin; // The point from which the view originates (front of the car)

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
                    Tile tile = (Tile)roadTiles.GetTile(cellPosition);
                    Vector3 point = roadTiles.GetCellCenterWorld(cellPosition);

                    // I figure real world nav systems could use CV to determine something like this
                    // so, rather than check with raycasts for every spot to see it's direction
                    // I'll cut some computation down and just check when I know it isn't a straight shot
                    if (tile.sprite.name == "Square")
                    {
                        Debug.Log("Square");
                        AddNode(point);
                    }
                    else 
                    {
                        Debug.Log("Not Square");
                        Bounds spriteBounds = tile.sprite.bounds;
                        Vector3 offset = spriteBounds.center; // Sprite offset relative to the grid center
                        Vector3 truePoint = point +  Vector3.Scale(offset, roadTiles.transform.localScale); // Adjust for tilemap scale
                        AddNode(truePoint);
                        PlaceNodesViaRaycasting(truePoint, 16);
                    }
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

    void PlaceNodesViaRaycasting(Vector3 tileCenter, int numRays)
    {
        if (distanceToNextCurvedNode == 0) {
            distanceToNextCurvedNode = .5f;
        }
        for (int i = 0; i < numRays; i++)
        {
            float angle = 2 * Mathf.PI * i / numRays;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            RaycastHit2D hit = Physics2D.Raycast(tileCenter, direction, curveMaxRadiusToConsider, roadMask);

            //TODO: Closer but fix this and maybe follow the hit for proper curve?
            if (hit.collider != null && roadTiles.HasTile(roadTiles.WorldToCell(hit.point)))
            {
                for (float distance = distanceToNextCurvedNode; distance <= curveMaxRadiusToConsider; distance += distanceToNextCurvedNode)
                {
                    Vector3 samplePoint = tileCenter + direction * distance;
                    RaycastHit2D sampleHit = Physics2D.Raycast(samplePoint, direction, curveMaxRadiusToConsider, roadMask);
                    if (sampleHit.collider != null)
                    {
                        AddNode(samplePoint);
                    }
                    // Validate the sampled point
                    /* if (roadTiles.HasTile(roadTiles.WorldToCell(samplePoint)))
                    {
                        AddNode(samplePoint);
                    } */
                }
                //AddNode(hit.point);
            }
        }


        /* float segmentAngle = 360 / (numRays - 1); // Adjust for the segments

        for (int i = 0; i < numRays; i++)
        {
            float angle = -360 / 2 + i * segmentAngle;
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * tileCenter;
            RaycastHit2D hit = Physics2D.Raycast(tileCenter, rayDirection, .1f, roadMask);
            if (hit.collider != null) 
            {
                for (float distance = .1f; distance <= maxRadius; distance += .1f)
                {
                    Vector3 samplePoint = tileCenter + rayDirection * distance;

                    // Validate the sampled point
                    if (roadTiles.HasTile(roadTiles.WorldToCell(samplePoint)))
                    {
                        AddNode(samplePoint);
                    }
                }
            } */

/*             if (hit.collider != null && roadTiles.HasTile(roadTiles.WorldToCell(hit.point)))
            {
                AddNode(hit.point);
            } */
        
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

                // TODO: Need something like raycasting to confirm the whole edge is on road
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

    void AStarPathCreation()
    {
        // get the node closest to the view origin in the manhattan direction of the target
        // get the target node as the node with closest target origin in the manhatan direction of car (viewOrigin)

        // create a priority que (whatever c# version is) for neighbors to search

        // create a set to look up nodes visited

        // start node value f(n) = 0 + h(n) where h(n) is the euclidean distance to target

        // add start node to nodes visited
        // add start node to priority que
        // targetFound = false

        // while priority que length > 0 and !targetFound
        
            // node = top of priority que
            // if node is target..... i'm at this step
            // for each neighbor to node 
                // add neighbor to visited set
                // find f(n) = current distance + euclidean to neighbor + euclidean to target
                // store in priority que if not in que

            



    }
}
