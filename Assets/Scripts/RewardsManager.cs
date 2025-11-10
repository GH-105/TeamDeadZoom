using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
        GameData data = SaveManager.LoadGame();
        
        if(data == null)
        {
            Debug.Log("No save data found!");
            return;
        }

        if(SceneManager.GetActiveScene().name == "Level 3")
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

        if(levelTime!= null)
        {
            gameManager.instance.currentTimeText.text = $"Current Time: {levelTime.currentTime:F2}s";
            gameManager.instance.bestTimeText.text = $"Best Time: {levelTime.bestTime:F2}s";
            gameManager.instance.outcomeText.text = $"You won {levelTime.levelName}";
        }
        else
        {
            gameManager.instance.currentTimeText.text = "Current Time: —";
            gameManager.instance.bestTimeText.text = "Best Time: —";
            gameManager.instance.outcomeText.text = $"You won {data.lastLevelCompleted}";
        }

        int coinsGained = Coinlogic.coinCount - coinsBeforeLevel;

        gameManager.instance.coinsGainedText.text = $"Coines Gained: {coinsGained} you have: {data.coins}"; 
        gameManager.instance.soulsGainedText.text = $"Souls: {data.souls}";

        

        gameManager.instance.rewardsPanel.SetActive(true);
        if(gameManager.instance.coinShopPanel != null)
            gameManager.instance.coinShopPanel.SetActive(false);
    }

    public void ShowCoinShop()
    {
        if (gameManager.instance.coinShopPanel != null && gameManager.instance.rewardsPanel != null)
        {
            gameManager.instance.rewardsPanel.SetActive(false);
            gameManager.instance.coinShopPanel.SetActive(true);
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
            gameManager.instance.currentTimeText.text = $"Current Time: {levelTime.currentTime:F2}s";
            gameManager.instance.bestTimeText.text = $"Best Time: {levelTime.bestTime:F2}s";
        }

        gameManager.instance.coinsGainedText.text = $"Coines Gained: {data.coins}";
        gameManager.instance.soulsGainedText.text = $"Souls Gained: {data.souls}";

        gameManager.instance.rewardsPanel.SetActive(true);
        if (gameManager.instance.coinShopPanel != null)
            gameManager.instance.coinShopPanel.SetActive(false);
        gameManager.instance.outcomeText.text = "You lost";
    }
}
