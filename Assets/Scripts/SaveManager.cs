using UnityEngine;
using System.IO;
using UnityEngine.Rendering;

public class SaveManager : MonoBehaviour
{
    private static string GetSavePath()
    {
        return Application.persistentDataPath + "/gamesave.json";
    }

    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log("game saved ");
    }
    public static GameData LoadGame()
    {
        string path = GetSavePath();
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("game loaded");
            return data;
        }
        return null;
    }
    public static bool SaveExists()
    {
        return File.Exists(GetSavePath());
    }
    public static void DeleteSave()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("deleted save");
        }
        if(StopWatch.instance != null)
        {
            StopWatch.instance.currentTime = 0f;
            StopWatch.instance.saveTime = float.MaxValue;
            StopWatch.instance.currentTimeText.text = "--:--:--";
            StopWatch.instance.saveTimeText.text = "--:--:--";

        }
    }

    public static float UpdateBestTime(string levelName, float currentTime)
    {
        GameData data = LoadGame();
        if(data == null)
            data = new GameData();

        bool levelFound = false;
        foreach(var level in data.levelTimes)
        {
            if(level.levelName == levelName)
            {
                levelFound = true;

                if (currentTime < level.bestTime || level.bestTime <= 0f)
                    level.bestTime = currentTime;
                

                level.currentTime = currentTime;
                level.enemiesKilled = gameManager.enemiesKilled;
                break;
            }
        }
        if(!levelFound)
        {
            GameData.LevelTimeData newLevelTime = new GameData.LevelTimeData
            {
                levelName = levelName,
                bestTime = currentTime,
                currentTime = currentTime
            };
           data.levelTimes.Add(newLevelTime);
        }
        data.lastLevelCompleted = levelName;

        SaveGame(data);
        return currentTime < float.MaxValue ? currentTime : float.MaxValue;

    }
   
}

 

