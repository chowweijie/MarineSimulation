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
            ShipInfo ship = new ShipInfo()
            {
                shipName = "Ship " + shipCount,
                speed = 100f,
                targetName = "Berth " + Random.Range(1, 38)
            };

            yield return new WaitForSeconds(spawnInterval);
            GameObject newShip = Instantiate(shipPrefab, GetRandomPosition(), Quaternion.identity);
            ShipController shipController = newShip.GetComponent<ShipController>();

            if (shipController != null)
            {
                shipController.SetShipData(ship);
                Debug.Log("Ship " + ship.shipName + " spawned!");
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

    Vector3 GetRandomPosition()
    {
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
