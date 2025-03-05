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
    private List<string> targetOptions = new List<string>();
    private int NoOfBerths = 38;
    private ShipInfo ship;
    
    public void SetShipInfo(ShipInfo shipRef){
        for (int i = 1; i <= NoOfBerths; i++)
        {
            targetOptions.Add("Berth " + i);
        }
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
