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
}
