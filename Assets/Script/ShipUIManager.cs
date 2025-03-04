using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUIManager : MonoBehaviour
{
    public GameObject shipItemUIPrefab;
    public Transform shipListContent;
    public List<ShipInfo> shipList = new List<ShipInfo>();

    // Start is called before the first frame update
    void Start()
    {
        AddShip();
        UpdateShipList();
    }

    public void AddShip(){
        ShipInfo newShip = new ShipInfo(){
            shipName = "Ship " + (shipList.Count + 1),
            speed = 1.0f,
            targetName = "Target 1"
        };
        shipList.Add(newShip);
        UpdateShipList();
    }

    public void RemoveShip(){
        if(shipList.Count > 0){
            shipList.RemoveAt(shipList.Count - 1);
            UpdateShipList();
        }
    }

    public void UpdateShipList(){
        foreach (Transform child in shipListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var ship in shipList)
        {
            ShipItemUI shipUI = Instantiate(shipItemUIPrefab, shipListContent).GetComponent<ShipItemUI>();
            shipUI.SetShipInfo(ship);
        }
    }
}
