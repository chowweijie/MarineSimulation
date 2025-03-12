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
            speed = 100f,
            targetName = "Berth 1"
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

    public void SaveShipList(){
        PlayerPrefs.SetInt("ShipCount", shipList.Count);
        for (int i = 0; i < shipList.Count; i++)
        {
            PlayerPrefs.SetString("ShipName" + i, shipList[i].shipName);
            PlayerPrefs.SetFloat("ShipSpeed" + i, shipList[i].speed);
            PlayerPrefs.SetString("ShipTarget" + i, shipList[i].targetName);
        }
        PlayerPrefs.Save();
    }
}
