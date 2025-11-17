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
    public float currentTime;
    public float saveTime = float.MaxValue;

    public TMP_Text currentTimeText;
    public TMP_Text saveTimeText;

    //private string sceneKey;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        currentTime = 0;

       // sceneKey = "BestTime_" + SceneManager.GetActiveScene().name; 

        StartStopwatch();
        UpdateBestTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatchActive && !gameManager.instance.isPaused)
        {
            currentTime = currentTime + Time.deltaTime;
        }
        TimeSpan cur = TimeSpan.FromSeconds(currentTime);
        currentTimeText.text = cur.ToString(@"mm\:ss\:ff");
    }

    public void StartStopwatch()
    {

        stopwatchActive = true;

    }
    public void StopStopwatch()
    {
        stopwatchActive = false;
        //SaveTime();
        //SaveTimeToSaveManager();
    } 
    /*
    public void SaveTime()
    {
        if (currentTime < saveTime)
        {
            saveTime = currentTime;
            PlayerPrefs.SetFloat(sceneKey, saveTime);
            PlayerPrefs.Save();
        }
        UpdateBestTime();
    }
    */
    public void UpdateBestTime()
    {
        GameData data = SaveManager.LoadGame();
        if (data != null)
        {
            saveTimeText.text = "--:--:--";
            return;
        }
        GameData.LevelTimeData leveltime = data.levelTimes.Find(lvl => lvl.levelName == SceneManager.GetActiveScene().name);

        if (leveltime != null)
            saveTimeText.text = TimeSpan.FromSeconds(leveltime.bestTime).ToString(@"mm\:ss\:ff");
        else
            saveTimeText.text = "--:--:--";
    }
    /*
    public void LoadTime()
    {
        saveTime = PlayerPrefs.GetFloat(sceneKey, float.MaxValue);
        UpdateBestTime();
    }
    public void DeleteTime()
    {
        PlayerPrefs.DeleteKey(sceneKey);
       
    }*/
    public void SaveTimeToSaveManager()
    {
        if (currentTime < 0f) return;

        SaveManager.UpdateBestTime(SceneManager.GetActiveScene().name, currentTime);
        UpdateBestTime();
    }
}
