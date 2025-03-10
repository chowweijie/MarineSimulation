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
    public float waypointDistance = 10f;
    private float turnSpeed = 50f;
    private bool isUnloading = false;
    public float nodeRadius = 100f;
    public ShipSpawner shipSpawner;
    private GameObject targetObject;
    private bool isExit = false;

    // Start is called before the first frame update
    void Start()
    {
        shipSpawner = FindObjectOfType<ShipSpawner>();
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
        Vector3 currentPosition = transform.position;
        currentPosition.y = 0;
        transform.position = currentPosition;

        if (path == null || path.Count == 0)
        {
            // Debug.Log("No path found");
            if (Vector3.Distance(transform.position, target.position) < nodeRadius && !isUnloading)
            {
                StartCoroutine(UnloadShip());
            }
            if (Vector3.Distance(transform.position, target.position) < nodeRadius*2 && isExit)
            {
                Debug.Log("Ship has exited the simulation!");
                FreeNode();
                Destroy(gameObject);
                Destroy(targetObject);
            }
            // Debug.Log("No path found" + gameObject.name);
            return;
        }

        FreeNode();

        if (targetIndex < path.Count) {
            Vector3 dir = path[targetIndex].worldPosition - transform.position;
            float angleDiff = Vector3.Angle(transform.forward, dir.normalized);
            dir.y = 0;
            // Debug.Log("Angle diff: " + angleDiff);
            if (Vector3.Distance(transform.position, path[targetIndex].worldPosition) < 2*waypointDistance)
            {
                targetIndex++;
            }
            else{
                if (angleDiff > 0.5f && !(Vector3.Distance(transform.position, path[targetIndex].worldPosition) < waypointDistance*2) && angleDiff != 180f)
                {
                    float rotationStep = Mathf.Min(angleDiff, turnSpeed * Time.deltaTime);
                    Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);
                }
                else
                {
                    transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
                }
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
        LayerMask unwalkableMask = LayerMask.GetMask("unwalkableMask");
        LayerMask berths = LayerMask.GetMask("Berths");
        Node node = grid.NodeFromWorldPoint(transform.position);
        if (!Physics.CheckSphere(node.worldPosition, nodeRadius, unwalkableMask))
        {
            node.occupied = false;
        }
        if (Physics.CheckSphere(node.worldPosition, nodeRadius, berths))
        {
            node.occupied = false;
        }
    }

    IEnumerator UnloadShip()
    {
        isUnloading = true;
        Debug.Log(gameObject.name + " has arrived at " + target.name + ". Unloading...");

        yield return new WaitForSeconds(5f);

        Debug.Log(gameObject.name + " finished unloading!");
        BerthManager.Instance.ReleaseBerth(target.name);

        targetObject = new GameObject("Target");
        targetObject.transform.position = shipSpawner.GetRandomPosition();
        target = targetObject.transform;
        isExit = true;
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            // Set Gizmos color to highlight the target node (e.g., green)
            Gizmos.color = Color.green;

            // Draw a sphere at the target node's world position
            if (path != null && path.Count > 0 && targetIndex < path.Count)
            {
                Gizmos.DrawSphere(path[targetIndex].worldPosition, 50f);
            }
            Gizmos.DrawSphere(target.position,nodeRadius/2); // Radius 0.5f (adjust as needed)
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
