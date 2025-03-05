using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public GameObject shipPrefab;
    public BoxCollider spawnArea;
    private List<ShipInfo> shipList = new List<ShipInfo>();

    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        SpawnShips();
    }
    
    void LoadData()
    {
        int shipCount = PlayerPrefs.GetInt("ShipCount", 0);
        for (int i = 0; i < shipCount; i++)
        {
            ShipInfo newShip = new ShipInfo()
            {
                shipName = PlayerPrefs.GetString("ShipName" + i, "Ship " + i),
                speed = PlayerPrefs.GetFloat("ShipSpeed" + i, 1.0f),
                targetName = PlayerPrefs.GetString("ShipTarget" + i, "Berth 1")
            };
            shipList.Add(newShip);
        }
    }

    void SpawnShips()
    {
        foreach (var ship in shipList)
        {
            GameObject newShip = Instantiate(shipPrefab, GetRandomPosition(), Quaternion.identity);
            
            ShipController shipController = newShip.GetComponent<ShipController>();
            if (shipController != null)
            {
                shipController.SetShipData(ship);
                Debug.Log("Ship " + ship.shipName + " spawned!");
            }
            else
            {
                Debug.LogError("ShipController not found in the ship prefab!");
            }
        }
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
