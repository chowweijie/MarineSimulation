using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TicksLeftUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    private float elapsedTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        int hours = Mathf.FloorToInt(elapsedTime / 3600);
        int minutes = Mathf.FloorToInt((elapsedTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timeText.text = $"Time: {hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
