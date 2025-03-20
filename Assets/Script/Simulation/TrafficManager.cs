using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrafficManager : MonoBehaviour
{
    private Queue<string> Bay1IncomingQueue = new Queue<string>();
    private Queue<string> Bay1OutgoingQueue = new Queue<string>();
    private Queue<string> Bay2IncomingQueue = new Queue<string>();
    private Queue<string> Bay2OutgoingQueue = new Queue<string>();
    private Queue<string> Bay3IncomingQueue = new Queue<string>();
    private Queue<string> Bay3OutgoingQueue = new Queue<string>();
    public bool lastBay1Incoming = true;
    public bool lastBay2Incoming = true;
    public bool lastBay3Incoming = true;
    public float trafficInterval = 5f;
    private List<string> approvedShips1 = new List<string>();
    private List<string> approvedShips2 = new List<string>();
    private List<string> approvedShips3 = new List<string>();
    public List<string> deniedShips = new List<string>();
    private List<ShipDelayInfo> shipDelayList = new List<ShipDelayInfo>();

    public struct ShipDelayInfo
    {
        public string ShipName;
        public string BerthName;
        public float DelayTime;
        public float TotalTime;

        public ShipDelayInfo(string shipName, float delayTime, float totalTime, string berthName)
        {
            ShipName = shipName;
            BerthName = berthName;
            DelayTime = delayTime;
            TotalTime = totalTime;
        }
    }

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
        if(approvedShips1.Contains(ship)){
            approvedShips1.Remove(ship);
            return true;
        }
        else if(approvedShips2.Contains(ship)){
            approvedShips2.Remove(ship);
            return true;
        }
        else if(approvedShips3.Contains(ship)){
            approvedShips3.Remove(ship);
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
        if(approvedShips1.Contains(ship)){
            approvedShips1.Remove(ship);
            StartCoroutine(TrafficInterval(1, 5));
            return true;
        }
        else if(approvedShips2.Contains(ship)){
            approvedShips2.Remove(ship);
            StartCoroutine(TrafficInterval(2, 5));
            return true;
        }
        else if(approvedShips3.Contains(ship)){
            approvedShips3.Remove(ship);
            StartCoroutine(TrafficInterval(3, 5));
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

    public void FreeQueue(string bay)
    {
        if (bay == "Bay 1"){
            lastBay1Incoming = true;
        }
        else if (bay == "Bay 2"){
            lastBay2Incoming = true;
        }
        else if (bay == "Bay 3"){
            lastBay3Incoming = true;
        }
    }

    private IEnumerator TrafficInterval(int bay, int delay){
        yield return new WaitForSeconds(delay);
        if (bay == 1){
            lastBay1Incoming = true;
        }
        else if (bay == 2){
            lastBay2Incoming = true;
        }
        else if (bay == 3){
            lastBay3Incoming = true;
        }
    }

    private void UpdateTraffic(){
        if((Bay1IncomingQueue.Count > 0 || Bay1OutgoingQueue.Count > 0) && approvedShips1.Count == 0 && lastBay1Incoming){
            if(Bay1IncomingQueue.Count > Bay1OutgoingQueue.Count){
                string ship = Bay1IncomingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips1.Add(ship);
                lastBay1Incoming = false;
            }
            else{
                string ship = Bay1OutgoingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips1.Add(ship);
                lastBay1Incoming = false;
            }
        }

        if((Bay2IncomingQueue.Count > 0 || Bay2OutgoingQueue.Count > 0) && approvedShips2.Count == 0 && lastBay2Incoming){
            if(Bay2IncomingQueue.Count > Bay2OutgoingQueue.Count){
                string ship = Bay2IncomingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips2.Add(ship);
                lastBay2Incoming = false;
            }
            else{
                string ship = Bay2OutgoingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips2.Add(ship);
                lastBay2Incoming = false;
            }
        }

        if((Bay3IncomingQueue.Count > 0 || Bay3OutgoingQueue.Count > 0) && approvedShips3.Count == 0 && lastBay3Incoming){
            if(Bay3IncomingQueue.Count > Bay3OutgoingQueue.Count){
                string ship = Bay3IncomingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips3.Add(ship);
                lastBay3Incoming = false;
            }
            else{
                string ship = Bay3OutgoingQueue.Dequeue();
                deniedShips.Remove(ship);
                approvedShips3.Add(ship);
                lastBay3Incoming = false;
            }
        }
    }

    public void RegisterShipDelay(string ship, float delay, float totalTime, string berth)
    {
        shipDelayList.Add(new ShipDelayInfo(ship, delay, totalTime, berth));
        SaveToFile(ship, delay, totalTime, berth);
    }

    private void SaveToFile(string ship, float delay, float totalTime, string berth)
    {
        string path = Application.dataPath + "/Logs/ShipDelay.txt";  // Saves in "Assets/Logs/"
        string entry = $"Ship: {ship}, Berth: {berth}, Delay: {delay} seconds, Total Time: {totalTime}\n";
        // Ensure the directory exists
        Directory.CreateDirectory(Application.dataPath + "/Logs/");
        System.IO.File.AppendAllText(path, entry);

        Debug.Log("File saved at: " + path);
        Debug.Log("Ship Delay saved to file");
    }
}
