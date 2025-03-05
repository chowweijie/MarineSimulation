using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShipItemUI : MonoBehaviour
{
    public TMP_InputField nameInput;
    public Slider speedInput;
    public TMP_Dropdown targetDropdown;
    public Button saveButton;
    private List<string> targetOptions = new List<string>(){
            "Berth 1",
            "Berth 2",
            "Berth 3",
            "Berth 4",
            "Berth 5"
    };
    private ShipInfo ship;
    
    public void SetShipInfo(ShipInfo shipRef){
        if(shipRef == null){
            Debug.LogError("ShipInfo is null");
            return;
        }
        ship = shipRef;
        nameInput.text = ship.shipName;
        speedInput.value = ship.speed;
        targetDropdown.ClearOptions();
        targetDropdown.AddOptions(targetOptions);
        targetDropdown.value = targetOptions.IndexOf(ship.targetName);
        saveButton.onClick.AddListener(UpdateShipInfo);
    }

    private void UpdateShipInfo(){
        ship.shipName = nameInput.text;
        ship.speed = speedInput.value;
        ship.targetName = targetDropdown.options[targetDropdown.value].text;
    }
}
