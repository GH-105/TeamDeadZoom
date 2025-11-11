using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEditor;

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
        DontDestroyOnLoad(gameObject);
    }
    public void StartLevel()
    {
        coinsBeforeLevel = Coinlogic.coinCount;
    }
    public void WinRewards()
    {
        Debug.Log("Rewards Manager called1");
        GameData data = SaveManager.LoadGame();
        Debug.Log("Rewards Manager called2");
        StopWatch.instance.UpdateBestTime();
        Debug.Log("Rewards Manager called3");
        if (data == null)
        {
            Debug.Log("No save data found!4");
            return;
        }
        Debug.Log("Rewards Manager called5");
        if (SceneManager.GetActiveScene().name == "Level 3")
            gameManager.instance.lastLevelText.text = "Level Completed: " + data.lastLevelCompleted + "\n congrats you win!";
        else
            gameManager.instance.lastLevelText.text = "Level Completed: " + data.lastLevelCompleted;
        Debug.Log("Rewards Manager called6");
        GameData.LevelTimeData levelTime = null;
        foreach( var lvl in data.levelTimes)
        {
            if(lvl.levelName == data.lastLevelCompleted)
            {
                levelTime = lvl; 
                break;
            }
        }
        Debug.Log("Rewards Manager called7");
        if (levelTime!= null)
        {
            gameManager.instance.currentTimeText.text = $"Current Time: {levelTime.currentTime:F2}s";
            gameManager.instance.bestTimeText.text = $"Best Time: {levelTime.bestTime:F2}s";
            gameManager.instance.outcomeText.text = $"You won {levelTime.levelName}";
        }
        else
        {
            gameManager.instance.currentTimeText.text = $"Current Time: {levelTime.currentTime:F2}s";
            gameManager.instance.bestTimeText.text = "Best Time: —";
            gameManager.instance.outcomeText.text = $"You won {data.lastLevelCompleted}";
        }
        Debug.Log("Rewards Manager called-Coin");
        int coinsGained = Coinlogic.coinCount - coinsBeforeLevel;
        Debug.Log("Rewards Manager called8");
        gameManager.instance.coinsGainedText.text = $"Coines Gained: {coinsGained} you have: {data.coins}"; 
        gameManager.instance.soulsGainedText.text = $"Souls: {data.souls}";


        Debug.Log("Rewards Manager called9");
        gameManager.instance.rewardsPanel.SetActive(true);
        if(gameManager.instance.coinShopPanel != null)
            gameManager.instance.coinShopPanel.SetActive(false);
        Debug.Log("Rewards Manager called10");
    }

    public void ShowCoinShop()
    {
        if (gameManager.instance.coinShopPanel != null && gameManager.instance.rewardsPanel != null)
        {
            gameManager.instance.rewardsPanel.SetActive(false);
            gameManager.instance.coinShopPanel.SetActive(true);
            gameManager.instance.menuActive = gameManager.instance.coinShopPanel;
        }
    }

    public void LossRewards()
    {
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
            gameManager.instance.currentTimeText.text = $"Current Time: {StopWatch.instance.currentTimeText.text}";
            gameManager.instance.bestTimeText.text = $"Best Time: {StopWatch.instance.saveTimeText.text}";
            gameManager.instance.outcomeText.text = $"You Lost {data.lastLevelCompleted}";
        }

        gameManager.instance.coinsGainedText.text = $"Coins Gained: {data.coins}";
        gameManager.instance.soulsGainedText.text = $"Souls Gained: {data.souls}";

        gameManager.instance.rewardsPanel.SetActive(true);
        if (gameManager.instance.coinShopPanel != null)
            gameManager.instance.coinShopPanel.SetActive(false);
        gameManager.instance.outcomeText.text = "You lost";
    }
}
