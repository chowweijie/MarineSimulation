using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public GameObject shipPrefab;
    public BoxCollider spawnArea;
    private List<ShipInfo> shipList = new List<ShipInfo>();
    private bool isSpawning = false;
    private float lambda = 0.02f;
    int[] possibleSize = { 100, 200, 300, 400 };
    private Quaternion rotation = Quaternion.Euler(0, 90, 0);
    public Transform leftSpawnArea;
    public Transform rightSpawnArea;
    private int maxTicks = 10000;

    // Start is called before the first frame update
    void Start()
    {
        isSpawning = true;
        SpawnStartShips();
        StartCoroutine(SpawnShips());
    }

    void SpawnStartShips()
    {
        int totalBerths = BerthManager.Instance.GetTotalBerths();
        int shipCount = totalBerths / 2;

        for (int i = 0; i < shipCount; i++)
        {
            bool atBerth = Random.value > 0.2f;
            string targetBerth = BerthManager.Instance.GetAvailableBerth();
            ShipInfo ship = new ShipInfo()
            {
                shipName = "Starter Ship " + i,
                speed = 30*30/3.6f,
                targetName = targetBerth
            };

            GameObject newShip = Instantiate(shipPrefab, GetRandomStarterPosition(targetBerth, atBerth), rotation);
            ShipController shipController = newShip.GetComponent<ShipController>();

            if (atBerth)
            {
                shipController.SetBerthSpawn();
            }

            if (shipController != null)
            {
                shipController.SetShipData(ship);
                Debug.Log("Starter Ship " + ship.shipName + " spawned!");
            }
            else
            {
                Debug.LogError("ShipController not found in the ship prefab!");
            }
        }
    }

    IEnumerator SpawnShips()
    {
        float spawnInterval = -Mathf.Log(1 - Random.value) / lambda;
        int shipCount = 1;
        int tickCount = 0;
        float tickSinceSpawn = 0;
        Debug.Log(maxTicks);
        Debug.Log(spawnInterval);

        while (isSpawning && tickCount < maxTicks)
        {
            if (tickSinceSpawn > spawnInterval)
            {
                spawnInterval = -Mathf.Log(1 - Random.value) / lambda;
                tickSinceSpawn = 0;
                Debug.Log("Spawning ship " + shipCount + " in " + spawnInterval + " ticks");

                string targetBerth = BerthManager.Instance.GetAvailableBerth();
                ShipInfo ship = new ShipInfo()
                {
                    shipName = "Ship " + shipCount,
                    speed = 30*30/3.6f,
                    targetName = targetBerth
                };

                GameObject newShip = Instantiate(shipPrefab, GetRandomSpawnPosition(), rotation);
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
            }
            
            yield return new WaitForSeconds(1);
            tickCount++;
            tickSinceSpawn++;
        }

        Debug.Log("Simulation ended!");
    }

    public Vector3 GetRandomSpawnPosition()
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

    public Vector3 GetRandomStarterPosition(string targetBerth, bool atBerth)
    {
        if (atBerth)
        {
            Transform target;
            target = GameObject.Find(targetBerth).transform;
            Vector3 targetPosition = target.position;
            return targetPosition;
        }
        else
        {
            return GetRandomSpawnPosition();
        }
    }
}
