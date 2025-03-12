using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public GameObject shipPrefab;
    public BoxCollider spawnArea;
    private List<ShipInfo> shipList = new List<ShipInfo>();
    private bool isSpawning = false;
    private float lambda = 0.1f;
    private int maxTicks = 10;
    int[] possibleSize = { 100, 200, 300, 400 };
    private Quaternion rotation = Quaternion.Euler(0, 90, 0);
    public Transform leftSpawnArea;
    public Transform rightSpawnArea;

    // Start is called before the first frame update
    void Start()
    {
        isSpawning = true;
        StartCoroutine(SpawnShips());
    }

    IEnumerator SpawnShips()
    {
        float spawnInterval = -Mathf.Log(1 - Random.value) / lambda;
        int shipCount = 1;
        int tickCount = 0;
        Debug.Log(maxTicks);

        while (isSpawning && tickCount < maxTicks)
        {
            string targetBerth = BerthManager.Instance.GetAvailableBerth();
            ShipInfo ship = new ShipInfo()
            {
                shipName = "Ship " + shipCount,
                speed = 200f,
                targetName = targetBerth
            };

            yield return new WaitForSeconds(spawnInterval);

            GameObject newShip = Instantiate(shipPrefab, GetRandomPosition(), rotation);
            float zScale = possibleSize[Random.Range(0, possibleSize.Length)];
            newShip.transform.localScale = new Vector3(50, 15, zScale);
            ShipController shipController = newShip.GetComponent<ShipController>();

            if (shipController != null)
            {
                shipController.SetShipData(ship);
                Debug.Log("Ship " + ship.shipName + " spawned!" + zScale);
                shipCount+=1;
            }
            else
            {
                Debug.LogError("ShipController not found in the ship prefab!");
            }
            tickCount++;
        }

        Debug.Log("Simulation ended!");
    }

    public Vector3 GetRandomPosition()
    {
        if(Random.value > 0.5f){
                spawnArea = leftSpawnArea.GetComponent<BoxCollider>();
                rotation = Quaternion.Euler(0, 90, 0);
            } else {
                spawnArea = rightSpawnArea.GetComponent<BoxCollider>();
                rotation = Quaternion.Euler(0, -90, 0);
            }
        if (spawnArea == null)
        {
            Debug.LogError("Spawn Area is not set!");
            return Vector3.zero;
        }
        Vector3 center = spawnArea.transform.position;
        Vector3 size = spawnArea.size;

        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, center.y, z);
    }
}
