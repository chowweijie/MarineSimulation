using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public Transform target;
    public AStarGrid grid;
    private PathFinding pathFinder;
    private List<Node> path = new List<Node>();
    private int targetIndex;
    public float speed = 1f;
    public float waypointDistance = 0.5f;
    private float turnSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null) {
        Debug.LogError("Target not assigned in the ShipController!");
        }
        grid = FindObjectOfType<AStarGrid>();
        if (grid == null) {
        Debug.LogError("Grid not assigned in the ShipController!");
        }
        OccupyNode();
        pathFinder = FindObjectOfType<PathFinding>();
        InvokeRepeating("UpdatePath", 0f, 5f);
    }

    public void SetShipData(ShipInfo shipData)
    {
        if (GameObject.Find(shipData.targetName) == null)
        {
            Debug.LogError(shipData.targetName + " is null");
            return;
        }
        gameObject.name = shipData.shipName;
        target = GameObject.Find(shipData.targetName).transform;
        speed = shipData.speed;
    }

    void UpdatePath()
    {
        // Debug.Log("Updating path");
        path = pathFinder.FindPath(transform.position, target.position);
        targetIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null || path.Count == 0)
        {
            // Debug.Log("No path found");
            return;
        }

        FreeNode();

        if (targetIndex < path.Count) {
            Vector3 dir = path[targetIndex].worldPosition - transform.position;
            float angleDiff = Vector3.Angle(transform.forward, dir.normalized);
            // Debug.Log("Angle diff: " + angleDiff);
            if (angleDiff > 0.5f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
            }
            if (Vector3.Distance(transform.position, path[targetIndex].worldPosition) < waypointDistance)
            {
                targetIndex++;
            }
        }

        OccupyNode();
    }

    void OccupyNode()
    {
        Node node = grid.NodeFromWorldPoint(transform.position);
        node.occupied = true;
    }

    void FreeNode()
    {
        Node node = grid.NodeFromWorldPoint(transform.position);
        node.occupied = false;
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            // Set Gizmos color to highlight the target node (e.g., green)
            Gizmos.color = Color.green;

            // Draw a sphere at the target node's world position
            if (path != null && path.Count > 0)
            {
                Gizmos.DrawSphere(path[targetIndex].worldPosition, 50f);
            }
            Gizmos.DrawSphere(target.position,50f); // Radius 0.5f (adjust as needed)
        }

        if (path != null)
        {
            for (int i = targetIndex; i < path.Count; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i].worldPosition, Vector3.one * 0.1f);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i].worldPosition);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1].worldPosition, path[i].worldPosition);
                }
            }
        }
    }
}
