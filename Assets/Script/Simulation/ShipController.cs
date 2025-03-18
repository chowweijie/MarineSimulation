using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public Transform target;
    public Transform berth;
    public AStarGrid grid;
    private PathFinding pathFinder;
    private List<Node> path = new List<Node>();
    private int targetIndex;
    public float speed = 1f;
    public float waypointDistance = 10f;
    private float turnSpeed = 0.5f;
    private float turnRate = 0.1f;
    private bool isUnloading = false;
    public float nodeRadius = 100f;
    public ShipSpawner shipSpawner;
    public BayManager bayManager;
    public TrafficManager trafficManager;
    private GameObject targetObject;
    private bool isExit = false;
    private bool entryPermitted = false;
    private string lane = "incoming";
    private string bay = "none";
    public float maxTurnAngle = 30f;  // Maximum rudder turn angle
    public float movementSpeed = 100f; // Forward speed
    public float rudderSensitivity = 1f; // Sensitivity for steering

    private float currentTurnAngle = 0f; // The current rudder angle

    // Start is called before the first frame update
    void Start()
    {
        bayManager = FindObjectOfType<BayManager>();
        shipSpawner = FindObjectOfType<ShipSpawner>();
        trafficManager = FindObjectOfType<TrafficManager>();
        if (target == null) {
        Debug.LogError("Target not assigned in the ShipController!");
        }
        grid = FindObjectOfType<AStarGrid>();
        if (grid == null) {
        Debug.LogError("Grid not assigned in the ShipController!");
        }
        OccupyNode();
        berth = target;
        if (!isUnloading) {
            GetBay();
        }
        pathFinder = FindObjectOfType<PathFinding>();
        InvokeRepeating("UpdatePath", 0f, 2f);
    }

    private void GetBay(){
        string[] parts = target.name.Split(' ');
        int num = int.Parse(parts[1]);
        if(num<3){
            bay = "none";
            entryPermitted = true;
            return;
        }
        else if(num<15){
            bay = "Bay 1";
        }
        else if(num<27){
            bay = "Bay 2";
        }
        else if(num<38){
            bay = "Bay 3";  
        }
        targetObject = new GameObject("Target");
        targetObject.transform.position = bayManager.GetRandomPosition(bay);
        target = targetObject.transform;
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
        path = pathFinder.FindPath(transform.position, target.position, lane, gameObject.name);
        targetIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isExit) {
            lane = "outgoing";
        }
        else {
            lane = "incoming";
        }
        Vector3 currentPosition = transform.position;
        currentPosition.y = 0;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        transform.position = currentPosition;

        if (Vector3.Distance(transform.position, target.position) < nodeRadius*3 && isExit)
        {
            Debug.Log("Ship has exited the simulation!");
            FreeNode();
            Destroy(gameObject);
            Destroy(targetObject);
            return;
        }

        if (Vector3.Distance(transform.position, target.position) < nodeRadius*2 && !isUnloading && !entryPermitted)
        {
            GetPermission();
            Debug.Log("Requesting to enter " + berth.name);
        }

        // UpdatePath();

        if (path == null || path.Count == 0)
        {
            // Debug.Log("No path found");
            if (Vector3.Distance(transform.position, berth.position) < nodeRadius*2 && !isUnloading)
            {
                StartCoroutine(UnloadShip());
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
                    // Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
                    // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

                    Vector3 forwardDirection = transform.forward;

                    // Find the angle difference between the current direction and target
                    float angleToTarget = Vector3.SignedAngle(forwardDirection, dir.normalized, Vector3.up);

                    // Adjust the rudder smoothly based on the target direction
                    // float turnInput = Mathf.Clamp(angleToTarget / maxTurnAngle, -1f, 1f);
                    // currentTurnAngle = Mathf.Lerp(currentTurnAngle, turnInput * maxTurnAngle, rudderSensitivity * Time.deltaTime);

                    // Apply gradual rotation like a real ship
                    transform.Rotate(Vector3.up, angleToTarget * turnSpeed * Time.deltaTime);

                    // Move forward with inertia
                    transform.Translate(forwardDirection * speed/2 * Time.deltaTime, Space.World);
                }
                else
                {
                    transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
                }
                
            }
        }

        OccupyNode();
    }

    void GetPermission()
    {
        if(trafficManager.RequestIncomingPermission(gameObject.name, bay)){
            entryPermitted = true;
            target = berth;
            Destroy(targetObject);
            Debug.Log("Permission granted to enter " + berth.name);
        }
        else{
            Debug.Log("Permission denied to enter " + berth.name);
        }
    }

    void OccupyNode()
    {
        Node node = grid.NodeFromWorldPoint(transform.position);
        node.occupiedBy = gameObject.name;

        foreach (Node neighbour in grid.GetNeighbours(node))
        {
            neighbour.occupiedBy = gameObject.name;
        }
    }

    void FreeNode()
    {
        LayerMask unwalkableMask = LayerMask.GetMask("unwalkableMask");
        LayerMask berths = LayerMask.GetMask("Berths");
        Node node = grid.NodeFromWorldPoint(transform.position);
        node.occupiedBy = "none";

        foreach (Node neighbour in grid.GetNeighbours(node))
        {
            neighbour.occupiedBy = "none";
        }
    }

    IEnumerator UnloadShip()
    {
        isUnloading = true;
        Debug.Log(gameObject.name + " has arrived at " + target.name + ". Unloading...");

        yield return new WaitForSeconds(5f);

        Debug.Log(gameObject.name + " finished unloading!");

        while(!trafficManager.RequestOutgoingPermission(gameObject.name, bay)){
            Debug.Log("Permission denied to exit " + berth.name);
            yield return new WaitForSeconds(2f);
        }
        Debug.Log("Permission granted to exit " + berth.name);
        BerthManager.Instance.ReleaseBerth(target.name);
        targetObject = new GameObject("Target");
        targetObject.transform.position = shipSpawner.GetRandomSpawnPosition();
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
