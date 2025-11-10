using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class StopWatch : MonoBehaviour
{
    public static StopWatch instance;
    bool stopwatchActive = false;
    float currentTime;
    public float saveTime = float.MaxValue;

    public TMP_Text currentTimeText;
    public TMP_Text saveTimeText;

    private string sceneKey;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        currentTime = 0;

        sceneKey = "BestTime_" + SceneManager.GetActiveScene().name; // get level name for each save
        Debug.Log(sceneKey);

        StartStopwatch();
        LoadTime();
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

        //Debug.Log("Time: " + currentTimeText.text);
    }

    public void StartStopwatch()
    {

        stopwatchActive = true;
        //Debug.Log("Stopwatch started");

    }
    public void StopStopwatch()
    {
        stopwatchActive = false;
        SaveTime();
    } 

    public void SaveTime()
    {
        if (currentTime < saveTime)
        {
            saveTime = currentTime;
            PlayerPrefs.SetFloat(sceneKey, saveTime);
            PlayerPrefs.Save();
            Debug.Log("new Best time");
        }
        UpdateBestTime();
    }

    public void UpdateBestTime()
    {
        if(saveTime == float.MaxValue)
        {
            saveTimeText.text = "--:--:--";
        }
        else
        {
            TimeSpan best = TimeSpan.FromSeconds(saveTime);
            saveTimeText.text = best.ToString(@"mm\:ss\:ff");
            
        }
        Debug.Log(sceneKey);
    }
    public void LoadTime()
    {
        saveTime = PlayerPrefs.GetFloat(sceneKey, float.MaxValue);
        UpdateBestTime();
    }
    public void DeleteTime()
    {
        PlayerPrefs.DeleteKey(sceneKey);
       
    }
    public void SaveTimeToSaveManager() // ill need later
    {
        if (currentTime < 0f) return;
        StopStopwatch();

        SaveManager.UpdateBestTime(SceneManager.GetActiveScene().name, currentTime);
    }
}
