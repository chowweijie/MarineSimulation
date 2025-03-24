using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerthManager : MonoBehaviour
{
    public static BerthManager Instance {get; private set;}

    public List<string> availableBerths = new List<string>();
    public List<string> occupiedBerths = new List<string>();

    public int maxBerths = 38;

    void Awake(){
        if(Instance == null){
            Instance = this;
        }else{
            Destroy(gameObject);
        }
        AddBerths();
    }

    void AddBerths(){
        for (int i = 1; i <= maxBerths; i++)
        {
            availableBerths.Add("Berth " + i);
        }
    }

    public int GetRemainingBerths(){
        return availableBerths.Count;
    }

    public string GetAvailableBerth(){
        if(availableBerths.Count > 0)
        {
            string berth = availableBerths[Random.Range(0, availableBerths.Count)];
            availableBerths.Remove(berth);
            occupiedBerths.Add(berth);
            // Debug.Log("Remaining Berths: " + availableBerths.Count);
            return berth;
        }
        else
        {
            Debug.Log("No available berth");
            return null;
        }
    }

    public void ReleaseBerth(string berth){
        if(occupiedBerths.Contains(berth)){
            occupiedBerths.Remove(berth);
            availableBerths.Add(berth);
        }
        // Debug.Log("Remaining Berths: " + availableBerths.Count);
    }
    public int GetTotalBerths(){
        return maxBerths;
    }
}
