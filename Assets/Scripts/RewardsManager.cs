using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif 
public class RewardsManager : MonoBehaviour
{
    public static RewardsManager instance;
    private int coinsBeforeLevel;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

 
    public void StartLevel()
    {
        coinsBeforeLevel = Coinlogic.coinCount;
    }
    public void WinRewards()
    {
        HideDoorMessage();
        GameData data = SaveManager.LoadGame();
        if (data == null)
        {
            Debug.Log("No save data found!4");
            return;
        }

        if (SceneManager.GetActiveScene().name == "Level 3")
            gameManager.instance.lastLevelText.text = "Level Completed: " + data.lastLevelCompleted + "\n congrats you win!";
        else
            gameManager.instance.lastLevelText.text = "Level Completed: " + data.lastLevelCompleted;

        GameData.LevelTimeData levelTime = null;
        foreach( var lvl in data.levelTimes)
        {
            if(lvl.levelName == data.lastLevelCompleted)
            {
                levelTime = lvl; 
                break;
            }
        }

        if (levelTime!= null)
        {
            TimeSpan cur = TimeSpan.FromSeconds(levelTime.currentTime);
            TimeSpan best = TimeSpan.FromSeconds(levelTime.bestTime);

            gameManager.instance.currentTimeText.text = "Current Time: " + cur.ToString(@"mm\:ss\:ff");
            gameManager.instance.bestTimeText.text = "Best Time: " + best.ToString(@"mm\:ss\:ff");

            if (Math.Abs(levelTime.currentTime - levelTime.bestTime) < .01f)
                gameManager.instance.outcomeText.text = $"First completion of {levelTime.levelName}";
            else
                gameManager.instance.outcomeText.text = "";
        }
        else
        {
           if(StopWatch.instance != null)
            {
                gameManager.instance.currentTimeText.text = StopWatch.instance.currentTimeText.text;
                gameManager.instance.bestTimeText.text = StopWatch.instance.saveTimeText.text;
            }
            else
            {
                gameManager.instance.currentTimeText.text = "--:--:--";
                gameManager.instance.bestTimeText.text = "--:--:--";
            }
            gameManager.instance.outcomeText.text = "";
        }
        int coinsGained = Coinlogic.coinCount - coinsBeforeLevel;
        gameManager.instance.coinsGainedText.text = $"Coins Gained: {coinsGained} you have: {data.coins}"; 
        gameManager.instance.soulsGainedText.text = $"Souls: {data.souls}";


        gameManager.instance.rewardsPanel.SetActive(true);
        if(gameManager.instance.coinShopPanel != null)
            gameManager.instance.coinShopPanel.SetActive(false);
        buttonFunctions.SaveGame(true);
    }

    public void ShowCoinShop()
    {
        Debug.Log("coin shop open");
        HideDoorMessage();
        if (gameManager.instance.coinShopPanel != null && gameManager.instance.rewardsPanel != null)
        {
            gameManager.instance.rewardsPanel.SetActive(false);
            gameManager.instance.coinShopPanel.SetActive(true);
            gameManager.instance.menuActive = gameManager.instance.coinShopPanel;
        }
    }

    public void LossRewards()
    {
        HideDoorMessage();

        GameData data = SaveManager.LoadGame();
        if (data == null)
        {
            Debug.Log("No save data found!");
            return;
        }

        gameManager.instance.lastLevelText.text = "Level Failed: " + data.lastLevelCompleted;

        GameData.LevelTimeData levelTime = null;
        foreach (var lvl in data.levelTimes)
        {
            if (lvl.levelName == data.lastLevelCompleted)
            {
                levelTime = lvl;
                break;
            }
        }
        if (levelTime != null)
        {
            Debug.Log("test 1 lose");
            gameManager.instance.currentTimeText.text = $"Current Time: {levelTime.currentTime:F2}s";
            gameManager.instance.bestTimeText.text = $"Best Time: {levelTime.bestTime:F2}s";
        }
        else
        {
            if (StopWatch.instance != null)
            {
                gameManager.instance.currentTimeText.text = StopWatch.instance.currentTimeText.text;
                gameManager.instance.bestTimeText.text = StopWatch.instance.saveTimeText.text;
            }
            else
            {
                gameManager.instance.currentTimeText.text = "--:--:--";
                gameManager.instance.bestTimeText.text = "--:--:--";
            }
        }

        gameManager.instance.coinsGainedText.text = $"Coins Gained: {data.coins}";
        gameManager.instance.soulsGainedText.text = $"Souls Gained: {data.souls}";

        gameManager.instance.outcomeText.text = "You lost";
        gameManager.instance.rewardsPanel.SetActive(true);
        if (gameManager.instance.coinShopPanel != null)
            gameManager.instance.coinShopPanel.SetActive(false);
    }

    private void HideDoorMessage()
    {
        room[] rooms = FindObjectsByType<room>(FindObjectsSortMode.None);
        foreach(room r in rooms)
        {
            if(r.doorStatusLabel != null)
                r.doorStatusLabel.gameObject.SetActive(false);
        }
    }

}

#if UNITY_EDITOR

public static class RewardsManagerDebug
{
    [MenuItem("Debug/Trigger Win screen")]
    private static void TriggerWin()
    {
        if (RewardsManager.instance == null)
        { 
            Debug.Log("Instance not in scene");
        return;
        }
        RewardsManager.instance.WinRewards();
    }

    [MenuItem("Debug/Trigger Loss screen")]
    private static void TriggerLoss()
    {
        if (RewardsManager.instance == null)
        { 
            Debug.Log("Instance not in scene");
        return;
    }
        RewardsManager.instance.LossRewards();
    }

    [MenuItem("Debug/Trigger souls")]
    private static void TriggerSouls()
    {
        SoulManagement.AddSouls(1);
    }
}

#endif