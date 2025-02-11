using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicGraphGenerator2D : MonoBehaviour
{
    public LayerMask roadMask;       // Layer for the road
    public Tilemap roadTiles;
    public float scanRadius = 2f;   // Scan radius around the agent
    public float nodeSpacing = 1f;   // Distance between nodes
    public float edgeConnectionDistance = 2f;  // Max distance to connect nodes
    public float curveMaxRadiusToConsider = 2f;
    public float distanceToNextCurvedNode = .75f;
    //public Transform viewOrigin; // The point from which the view originates (front of the car)
    public Transform destination; // the target destination in my mind an equivalent to coordinates or an address in maps

    private List<Vector2> graphNodes = new List<Vector2>();
    private List<(Vector2, Vector2)> graphEdges = new List<(Vector2, Vector2)>();
    private Dictionary<Vector2, List<(Vector2, Vector2)>> graphEdgeAdgency = new();

    private float currentStartNodeBestDistanceToCar = 10000000;
    private Vector2 startNode;
    private float currentStartNodeBestDistanceToTarget = 10000000;
    private Vector2 endNode;
    public List<Vector2> path = new();
    private List<Vector2> visitedNodes = new();

    void Start()
    {
        if (roadTiles == null) {
            roadTiles = GameObject.FindGameObjectWithTag("TrackTilemap").GetComponent<Tilemap>();
        }
        roadTiles.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
        GenerateGraph();
        ConnectNodes();
        AStarPathCreation();
        roadTiles.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Outlines;
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
                        AddNode(point);
                    }
                    else 
                    {
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
            float distanceToCar = Vector2.Distance(transform.position, point);
            if (distanceToCar < currentStartNodeBestDistanceToCar) {
                currentStartNodeBestDistanceToCar = distanceToCar;
                startNode = point;
            }

            float distanceToTarget = Vector2.Distance(destination.position, point);
            if (distanceToTarget < currentStartNodeBestDistanceToTarget) {
                currentStartNodeBestDistanceToTarget = distanceToTarget;
                endNode = point;
            }
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
                }
            }
        }
    }



    // Connect nearby nodes with edges
    void ConnectNodes()
    {
        graphEdges.Clear();  // Clear existing edges
        graphEdgeAdgency.Clear();
        visitedNodes.Clear();
        path.Clear();
        Debug.Log($"Connect Nodes graph nodes length: {graphNodes.Count}");
        for (int i = 0; i < graphNodes.Count; i++)
        {
            if (!graphEdgeAdgency.ContainsKey(graphNodes[i]))
            {
                graphEdgeAdgency.Add(graphNodes[i], new List<(Vector2, Vector2)>());
            }
            for (int j = i + 1; j < graphNodes.Count; j++)
            {
                // Connect nodes that are close enough (avoids cross-connecting far nodes)
                if (Vector2.Distance(graphNodes[i], graphNodes[j]) <= edgeConnectionDistance)
                {
                    graphEdges.Add((graphNodes[i], graphNodes[j]));
                    List<(Vector2, Vector2)> temp = graphEdgeAdgency[graphNodes[i]];
                    temp.Add((graphNodes[i], graphNodes[j]));
                    graphEdgeAdgency[graphNodes[i]] = temp;
                }
                
            }
        }
        Debug.Log($"End connect. Graph Adgency key count: {graphEdgeAdgency.Keys.Count}");
    }


    // Draw Gizmos for debugging
    void OnDrawGizmos()
    {
        if (roadTiles == null) {
            roadTiles = GameObject.FindGameObjectWithTag("TrackTilemap").GetComponent<Tilemap>();
        }
        roadTiles.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Polygons;
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

        if (path.Count == 0)
        {
            AStarPathCreation();
        }
        //Debug.Log($"Path steps: {path.Count}");
        Gizmos.color = Color.yellow;
        foreach(Vector2 node in path)
        {
            Gizmos.DrawCube((Vector3)node, new Vector3(.3f, .3f, 0));
            //Debug.Log($"Path step: {node}");
        }

        foreach(Vector2 node in visitedNodes)
        {
            Gizmos.DrawSphere((Vector3)node, .1f);
        }
        roadTiles.GetComponent<CompositeCollider2D>().geometryType = CompositeCollider2D.GeometryType.Outlines;
    }

    public class PathNode 
    {
        public Vector2 Position { get; set; }
        public PathNode Parent { get; set; }
        public float Gn { get; set; }
        public float Hn { get; set; }
        public float Fn => Gn + Hn;

        public override bool Equals(object obj)
        {
            return obj is PathNode node && Position.Equals(node.Position);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }

    // Custom comparer to sort nodes by fCost
    public class FCostComparer : IComparer<PathNode> 
    {
        public int Compare(PathNode a, PathNode b) {
            int compare = a.Fn.CompareTo(b.Fn);
            return compare == 0 ? a.Position.GetHashCode().CompareTo(b.Position.GetHashCode()) : compare;
        }
    }

    float SquaredEuclideanDistance(Vector2 pointA, Vector2 pointB) // for a slight speed pick up skipping the sqrt
    {
        float dx = pointB.x - pointA.x;
        float dy = pointB.y - pointA.y;
        return dx * dx + dy * dy;
    }

    public void AStarPathCreation()
    {
        // start node and target node are obtained in generating the graph
        // create a priority que (whatever c# version is) for neighbors to search
        SortedSet<PathNode> openSet = new(new FCostComparer());
        // create a set to look up nodes visited
        HashSet<Vector2> closedSet = new();
        Dictionary<Vector2, PathNode> nodeLookup = new();
        // start node value f(n) = 0 + h(n) where h(n) is the euclidean distance to target
        PathNode startPathNode = new()
        {
            Position = startNode,
            Parent = null,
            Gn = 0,
            Hn = Vector2.Distance(startNode, endNode)
        };
        // add start node to priority que
        openSet.Add(startPathNode);
        nodeLookup.Add(startNode, startPathNode);

        while(openSet.Count > 0) 
        {
            PathNode node = openSet.Min;
            openSet.Remove(node);
            nodeLookup.Remove(node.Position);
            visitedNodes.Add(node.Position);

            if (node.Position == endNode)
            {
                path.Add(node.Position);
                // trace the path back to the start by following the parent
                PathNode curr = node.Parent;
                while (curr != null)
                {
                    path.Add(curr.Position);
                    curr = curr.Parent;
                }
                // reverse "path"
                path.Reverse();
                return;
            }
            // add node to visited set
            closedSet.Add(node.Position);

            // for each neighbor to node 
            foreach ((Vector2, Vector2) pair in graphEdgeAdgency[node.Position])
            {
                // if neighbor not in visited set:
                if (!closedSet.Contains(pair.Item2))
                {
                    // create node with vector2, gn, fn, and parent
                    PathNode neighborNode = new()
                    {
                        Position = pair.Item2,
                        Parent = node,
                        Gn = node.Gn + Vector2.Distance(node.Position, pair.Item2), // g(n) = node.gn + euclidean to neighbor
                        Hn = Vector2.Distance(pair.Item2, endNode) // h(n) = neighbor euclidean to target
                        // f(n) = g(n) + h(n)
                    };
                    if (nodeLookup.ContainsKey(neighborNode.Position))
                    {
                        if (nodeLookup[neighborNode.Position].Gn > neighborNode.Gn) {
                            openSet.Remove(nodeLookup[neighborNode.Position]);
                            openSet.Add(neighborNode);
                            nodeLookup[neighborNode.Position] = neighborNode;
                        }
                    }
                    else
                    {
                        openSet.Add(neighborNode);
                        nodeLookup.Add(neighborNode.Position, neighborNode);
                    }
                }
            }
        }
        Debug.Log("No path found");
    }

}
