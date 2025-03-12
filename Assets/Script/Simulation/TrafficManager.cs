using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    private Queue<string> Bay1IncomingQueue = new Queue<string>();
    private Queue<string> Bay1OutgoingQueue = new Queue<string>();
    private Queue<string> Bay2IncomingQueue = new Queue<string>();
    private Queue<string> Bay2OutgoingQueue = new Queue<string>();
    private Queue<string> Bay3IncomingQueue = new Queue<string>();
    private Queue<string> Bay3OutgoingQueue = new Queue<string>();
    private bool lastBay1Incoming = false;
    private bool lastBay2Incoming = false;
    private bool lastBay3Incoming = false;
    public float trafficInterval = 5f;
    private List<string> approvedShips = new List<string>();
    private List<string> deniedShips = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateTraffic", 0f, trafficInterval);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool RequestIncomingPermission(string ship, string bay){
        if(approvedShips.Contains(ship)){
            approvedShips.Remove(ship);
            return true;
        }
        else if(deniedShips.Contains(ship)){
            return false;
        }
        else{
            if(bay == "Bay 1"){
                Bay1IncomingQueue.Enqueue(ship);
            }
            else if(bay == "Bay 2"){
                Bay2IncomingQueue.Enqueue(ship);
            }
            else if(bay == "Bay 3"){
                Bay3IncomingQueue.Enqueue(ship);
            }
            deniedShips.Add(ship);
            return false;
        }
    }

    public bool RequestOutgoingPermission(string ship, string bay){
        if(approvedShips.Contains(ship)){
            approvedShips.Remove(ship);
            return true;
        }
        else if(deniedShips.Contains(ship)){
            return false;
        }
        else{
            if(bay == "Bay 1"){
                Bay1OutgoingQueue.Enqueue(ship);
            }
            else if(bay == "Bay 2"){
                Bay2OutgoingQueue.Enqueue(ship);
            }
            else if(bay == "Bay 3"){
                Bay3OutgoingQueue.Enqueue(ship);
            }
            else if(bay == "none"){
                return true;
            }
            deniedShips.Add(ship);
            return false;
        }
    }

    private void UpdateTraffic(){
        if(Bay1IncomingQueue.Count > 0){
            if(lastBay1Incoming){
                string ship = Bay1IncomingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips.Add(ship);
                lastBay1Incoming = false;
            }
            else{
                lastBay1Incoming = true;
            }
        }
        if(Bay2IncomingQueue.Count > 0){
            if(lastBay2Incoming){
                string ship = Bay2IncomingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips.Add(ship);
                lastBay2Incoming = false;
            }
            else{
                lastBay2Incoming = true;
            }
        }
        if(Bay3IncomingQueue.Count > 0){
            if(lastBay3Incoming){
                string ship = Bay3IncomingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips.Add(ship);
                lastBay3Incoming = false;
            }
            else{
                lastBay3Incoming = true;
            }
        }
        if(Bay1OutgoingQueue.Count > 0){
            if(!lastBay1Incoming){
                string ship = Bay1OutgoingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips.Add(ship);
                lastBay1Incoming = false;
            }
            else{
                lastBay1Incoming = true;
            }
        }
        if(Bay2OutgoingQueue.Count > 0){
            if(!lastBay2Incoming){
                string ship = Bay2OutgoingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips.Add(ship);
                lastBay2Incoming = false;
            }
            else{
                lastBay2Incoming = true;
            }
        }
        if(Bay3OutgoingQueue.Count > 0){
            if(!lastBay3Incoming){
                string ship = Bay3OutgoingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips.Add(ship);
                lastBay3Incoming = false;
            }
            else{
                lastBay3Incoming = true;
            }
        }
    }
}
