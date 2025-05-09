using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Renderer>() == null)
        {
            Debug.LogError("AStarGrid needs a renderer component");
            return;
        }
        if (nodeRadius <= 0)
        {
            Debug.LogError("Node radius must be greater than 0");
            return;
        }
        Renderer renderer = GetComponent<Renderer>();
        gridWorldSize = new Vector2(renderer.bounds.size.x, renderer.bounds.size.z);
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        LayerMask unwalkableMask = LayerMask.GetMask("unwalkableMask");
        LayerMask berths = LayerMask.GetMask("Berths");
        LayerMask incoming = LayerMask.GetMask("Incoming");
        LayerMask outgoing = LayerMask.GetMask("Outgoing");
        LayerMask intersection = LayerMask.GetMask("Intersection");
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = true;
                string lane = "none";
                if (Physics.CheckSphere(worldPoint, nodeRadius, incoming))
                {
                    lane = "incoming";
                }
                else if (Physics.CheckSphere(worldPoint, nodeRadius, outgoing))
                {
                    lane = "outgoing";
                }
                if (Physics.CheckSphere(worldPoint, nodeRadius, intersection))
                {
                    lane = "none";
                }
                if (Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask))
                {
                    walkable = false;
                }
                if (Physics.CheckSphere(worldPoint, nodeRadius, berths))
                {
                    walkable = true;
                }
                grid[x, y] = new Node(walkable, worldPoint, x, y, lane);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if(grid[checkX, checkY].occupied)
                    {
                        continue;
                    }
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null) {
            foreach (Node n in grid) {
                Gizmos.color = (n.walkable && !n.occupied) ? Color.white : Color.red;
                Gizmos.DrawWireSphere(n.worldPosition, nodeRadius);
            }
            foreach (Node n in grid) {
                if (n.lane == "incoming")
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(n.worldPosition, nodeRadius);
                }
                else if (n.lane == "outgoing")
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(n.worldPosition, nodeRadius);
                }
                else if (n.occupiedBy.Contains("Ship"))
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(n.worldPosition, nodeRadius);
                }
            }
        }
        
    }
}
