using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RewardsManager : MonoBehaviour
{
    [SerializeField] public TMP_Text lastLevelText;
    [SerializeField] public TMP_Text currentTimeText;
    [SerializeField] public TMP_Text bestTimeText;
    [SerializeField] public TMP_Text coinsGainedText;
    [SerializeField] public TMP_Text soulsGainedText;
    [SerializeField] public TMP_Text outcomeText;

    [SerializeField] private GameObject rewardsCanvas;
    [SerializeField] private GameObject coinShopCanvas;

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

        coinsGainedText.text = $"Coines: {data.coins}"; // add a way to show how much it changed by this level
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
 * -add to game manager where enemy destroy is called-
 * public static void EnemyKillCount()
 * {
 * enemiesKilled++;
 * }
 *  -call lossRewards on playerdeath
 *  -call winRewards where the win screen would be called
 * 
 * -add to buttonFunctions -
 * public void nextLevel()
 * {
 * string currentScene = SceneManager.GetActiveScene().name;
 * 
 * switch (currentScene)
 * {
 *      case "Level 1":
 *          SceneManager.LoadScence("Level 2");
 *          break;
 *      case "Level 2":
 *          SceneManager.LoadScence("Level 3");
 *          break;
 *      case "Level 3":
 *          SceneManager.LoadScence("Level Select");
 *          break;
 *      default:
 *          Debug.Log("Unexpected Scene: " + currentScene);
 *          break;
 *          // add a win Menu for beating the game and toggle credits after case 3
 *   }
 *   gameManager.instance.stateUnpause();
 *   }
 * 
 *
 * }
 * 
 * -add to gameManager-
 * [SerializeField] private GameObject reticle;
 * 
 * public void HideReticle()
 * {
 *  if(reticle != null)
 *      {
 *      reticle.SetActive(false);
 *      }
 * }
 * 
 *  * public void ShowReticle()
 * {
 *  if(reticle != null)
 *      {
 *      reticle.SetActive(true);
 *      }
 * }
 * 
 * 
 **/