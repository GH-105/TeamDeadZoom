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
        Debug.Log("[SAVE] Writing save file...");
        Debug.Log("[SAVE] Souls=" + data.souls +
            " Coins=" + data.coins +
            " BestTimesCount=" + data.levelTimes.Count +
            " HardModeSelected= " + data.HardModeSelected);

        string json = JsonUtility.ToJson(data, true);

        Debug.Log("[SAVE] JSON Contents:\n" + json);

        File.WriteAllText(GetSavePath(), json);

        Debug.Log("[SAVE] File written to: " + GetSavePath());
    }
    public static GameData LoadGame()
    {
        string path = GetSavePath();
        if(File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("[LOAD] JSON read:\n" + json);

            GameData data = JsonUtility.FromJson<GameData>(json);

            Debug.Log("[LOAD] Loaded -> Souls=" + data.souls +
                " Coins=" + data.coins +
                " BestTimes=" + data.levelTimes.Count +
                " HardModeSelected=" + data.HardModeSelected);

            return data;
        }

        Debug.Log("[LOAD] Save file does not exist. ");
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
        Debug.Log("[TIME] Updating best time for level: " + levelName);

        GameData data = LoadGame();
        Debug.Log("[TIME] Before update: " + data.levelTimes.Count + " entries");
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

        Debug.Log("[TIME] After update: " + data.levelTimes.Count + " entries");

        SaveGame(data);
        return currentTime < float.MaxValue ? currentTime : float.MaxValue;

    }
   
}

 

