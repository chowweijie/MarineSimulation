using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BayManager : MonoBehaviour
{
    public Transform Bay1;
    public Transform Bay2;
    public Transform Bay3;
    public BoxCollider spawnArea;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetRandomPosition(string bay)
    {
        if(bay == "Bay 1"){
                spawnArea = Bay1.GetComponent<BoxCollider>();
            }
            else if(bay == "Bay 2"){
                spawnArea = Bay2.GetComponent<BoxCollider>();
            }
            else if(bay == "Bay 3"){
                spawnArea = Bay3.GetComponent<BoxCollider>();
            }
        if (spawnArea == null)
        {
            Debug.LogError("Bay Area is not set!");
            return Vector3.zero;
        }
        Vector3 center = spawnArea.transform.position;
        Vector3 size = spawnArea.size;

        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, center.y, z);
    }
}
