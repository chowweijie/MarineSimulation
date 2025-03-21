using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipSpawner : MonoBehaviour
{
    public GameObject shipPrefab;
    public BoxCollider spawnArea;
    private List<ShipInfo> shipList = new List<ShipInfo>();
    private bool isSpawning = false;
    private float lambda = 0.016f;
    int[] possibleSize = { 100, 200, 300, 400 };
    private Quaternion rotation = Quaternion.Euler(0, 90, 0);
    public Transform leftSpawnArea;
    public Transform rightSpawnArea;
    private int maxTicks = 14400;
    private float shipSpeed = 20*30/3.6f;

    // Start is called before the first frame update
    void Start()
    {
        isSpawning = true;
        SpawnStartShips();
        StartCoroutine(SpawnShips());
    }

    private float GetRandomSize()
    {
        string[] possibleSize = { "120-180", "181-240", "241-320", "321-400", "401-450" };
        float[] probability = { 28.3f, 23.5f, 30.2f, 17.4f, 0.6f};
        float[] cumulativeProbability = new float[5];
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                cumulativeProbability[i] = probability[i];
            }
            else
            {
                cumulativeProbability[i] = cumulativeProbability[i - 1] + probability[i];
            }
        }

        float randomValue = Random.value * 100;
        for (int i = 0; i < 5; i++)
        {
            if (randomValue <= cumulativeProbability[i])
            {
                string[] range = possibleSize[i].Split('-');
                return Random.Range(float.Parse(range[0]), float.Parse(range[1]));
            }
        }

        Debug.LogError("Random size not found!");
        return 0;
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
                speed = shipSpeed,
                targetName = targetBerth
            };

            GameObject newShip = Instantiate(shipPrefab, GetRandomStarterPosition(targetBerth, atBerth), rotation);
            float zScale = GetRandomSize();
            newShip.transform.localScale = new Vector3(50, 15, zScale);
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
            if (BerthManager.Instance.GetRemainingBerths() == 0)
            {
                Debug.Log("No more berths available!");
                continue;
            }

            if (tickSinceSpawn > spawnInterval)
            {
                spawnInterval = -Mathf.Log(1 - Random.value) / lambda;
                tickSinceSpawn = 0;
                Debug.Log("Spawning ship " + shipCount + " in " + spawnInterval + " ticks");

                string targetBerth = BerthManager.Instance.GetAvailableBerth();
                ShipInfo ship = new ShipInfo()
                {
                    shipName = "Ship " + shipCount,
                    speed = shipSpeed,
                    targetName = targetBerth
                };

                GameObject newShip = Instantiate(shipPrefab, GetRandomSpawnPosition(), rotation);
                float zScale = GetRandomSize();
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
        SceneManager.LoadSceneAsync(0);
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
