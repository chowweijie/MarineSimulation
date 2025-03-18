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

    public string GetAvailableBerth(){
        if(availableBerths.Count > 0)
        {
            string berth = availableBerths[Random.Range(0, availableBerths.Count)];
            availableBerths.Remove(berth);
            occupiedBerths.Add(berth);
            return berth;
        }
        else
        {
            Debug.Log("No available berth");
            return null;
        }
    }

    public int GetTotalBerths(){
        return maxBerths;
    }

    public void ReleaseBerth(string berth){
        if(occupiedBerths.Contains(berth)){
            occupiedBerths.Remove(berth);
            availableBerths.Add(berth);
        }
    }
}
