using UnityEngine;
using System.IO;

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

                if(currentTime < level.bestTime)
                    level.bestTime = currentTime;
                

                level.currentTime = currentTime;
               // level.enemiesKilled = gameManagement.enemiesKilled;
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
/* add -public static int enemiesKilled = 0; to gameManager
 * 
 * -add this to gameManager-
 * public void ResetKillCount()
 * {
 *      enemiesKilled = 0;
 * }
 * 
 *  public void SaveGame()
    {
        GameData data = new GameData
        {
            souls = SoulManagement.souls,
            playerHP = gameManager.instance.playerScript.HP,
            checkpointPosition = gameManager.instance.playerSpawnPos.transform.position
        };
        data.gunData = new GameData.GunData[gameManager.instance.guns.Length];
        data.currentGunIndex = gameManager.instance.currentGunIndex;

        for(int i = 0; i < gameManager.instance.guns.Length; i++)
    {
            data.gunData[i] = new GameData.GunData
        {
            flatDamageMod = gameManager.instance.guns[i].flatDamageMod,
            damageMultMod = gameManager.instance.guns[i].damageMultMod,
            addMaxAmmoMod = gameManager.instance.guns[i].addMaxAmmoMod
        };
    }
        data.levelTimes = new List<GameData.LevelTimeData>(gameManager.instance.levelTimes);
        data.lastLevelCompleted = SceneManager.GetActiveScene().name;

        SaveManager.SaveGame(data);
    }
    public void LoadGame()
    {
        GameData data = SaveManager.LoadGame();
        if(data != null)
        {
            SoulManagement.souls = data.souls;
            Coinlogic.coinCount = data.coins;
            gameManager.instance.playerScript.HP = data.playerHP;
            gameManager.instance.playerSpawnPos.transform.position = data.checkpointPosition;
            gameManager.instance.playerScript.spawnPlayer();

            Debug.Log($"Loaded : {data.souls} souls, HP {data.playerHP}, checkpoint {data.checkpointPosition}");
            
            for(int i = 0; i < data.gunData.Length; i++)
        {
            gameManager.instance.guns[i].flatDamageMod = data.gunData[i].flatDamageMod;
            gameManager.instance.guns[i].damageMultMod = data.gunData[i].damageMultMod;
            gameManager.instance.guns[i].addMaxAmmoMod = data.gunData[i].addMaxAmmoMod;
        }
    }
 * */