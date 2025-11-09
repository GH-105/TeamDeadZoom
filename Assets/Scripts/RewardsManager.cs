using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager instance;
    [SerializeField] public TMP_Text lastLevelText;
    [SerializeField] public TMP_Text currentTimeText;
    [SerializeField] public TMP_Text bestTimeText;
    [SerializeField] public TMP_Text coinsGainedText;
    [SerializeField] public TMP_Text soulsGainedText;
    [SerializeField] public TMP_Text outcomeText;

    [SerializeField] private GameObject rewardsCanvas;
    [SerializeField] private GameObject coinShopCanvas;

    private int coinsBeforeLevel;

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
            lastLevelText.text = "Level Completed: " + data.lastLevelCompleted + "\n congrats you win!";
        else
            lastLevelText.text = "Level Completed: " + data.lastLevelCompleted;

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
            currentTimeText.text = $"Current Time: {levelTime.currentTime:F2}s";
            bestTimeText.text = $"Best Time: {levelTime.bestTime:F2}s";
        }

        int coinsGained = Coinlogic.coinCount - coinsBeforeLevel;

        coinsGainedText.text = $"Coines Gained: {coinsGained} you have: {data.coins}"; 
        soulsGainedText.text = $"Souls: {data.souls}";

        outcomeText.text = $"You won {levelTime.levelName}";

        rewardsCanvas.SetActive(true);
        if(coinShopCanvas != null) 
            coinShopCanvas.SetActive(false);
    }

    public void ShowCoinShop()
    {
        if(coinShopCanvas != null)
        {
            coinShopCanvas.SetActive(true);
            rewardsCanvas.SetActive(false);
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

        lastLevelText.text = "Level Failed: " + data.lastLevelCompleted;

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
            currentTimeText.text = $"Current Time: {levelTime.currentTime:F2}s";
            bestTimeText.text = $"Best Time: {levelTime.bestTime:F2}s";
        }

        coinsGainedText.text = $"Coines Gained: {data.coins}";
        soulsGainedText.text = $"Souls Gained: {data.souls}";

        rewardsCanvas.SetActive(true);
        if (coinShopCanvas != null)
            coinShopCanvas.SetActive(false);
        outcomeText.text = "You lost";
    }
}
/*
 * add to where win menu and lose menu is called when update from main
 * 
 * -RewardsScreen rewards = FindObjectOfType<RewardsScreen>();
 * -RewardsManager.ShowRewards();
 *  
 *  +call lossRewards on playerdeath
 *  -call winRewards where the win screen would be called
 * 
 *
 * 
 *
 **/