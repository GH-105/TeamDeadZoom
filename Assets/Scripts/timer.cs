using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class StopWatch : MonoBehaviour
{
    bool stopwatchActive = false;
    float currentTime;
    public TMP_Text currentTimeText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = 0;
        StartStopwatch();
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatchActive && !gameManager.instance.isPaused)
        {
            currentTime = currentTime + Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        currentTimeText.text = time.ToString(@"mm\:ss\:ff");

        Debug.Log("Time: " + currentTimeText.text);
    }

    public void StartStopwatch()
    {

        stopwatchActive = true;
        Debug.Log("Stopwatch started");

    }
    public void StopStopwatch()
    {
        stopwatchActive = false;
    }
}
