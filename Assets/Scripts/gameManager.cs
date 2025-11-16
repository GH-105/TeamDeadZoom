using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Playables;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] public GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuRoomComp;
    [SerializeField] GameObject menuLevelSelect;
    [SerializeField] GameObject menuWeaponSelect;
    [SerializeField] GameObject EnemyAuraScreenOverlay;
    [SerializeField] GameObject LavaScreenOverlay;
    [SerializeField] public GameObject playerUWScreen;
    [SerializeField] public Camera HpCamera;
    [SerializeField] public GameObject StartMenu;
    [SerializeField] public GameObject OptionsMenu;
    [SerializeField] public GameObject SoulShopMenu;
    [SerializeField] public GameObject TutorialButton;
    [SerializeField] public GameObject reloadText;
    [SerializeField] public TMP_Text lastLevelText;
    [SerializeField] public TMP_Text currentTimeText;
    [SerializeField] public TMP_Text bestTimeText;
    [SerializeField] public TMP_Text coinsGainedText;
    [SerializeField] public TMP_Text soulsGainedText;
    [SerializeField] public TMP_Text outcomeText;
    [SerializeField] public List<hearts> playerHearts;
    [SerializeField] public room bossRoom;
    [SerializeField] public DamageDirection DamageIndicatorDir;
    [SerializeField] public int finalLevelIndex = 3;
    [SerializeField] GameObject hardModeButton;
    [SerializeField] public PlayableDirector endingCutscene;

    [SerializeField] public GameObject rewardsPanel;
    [SerializeField] public GameObject coinShopPanel;

    public gunStats startingGun;
    [SerializeField] public Slider ReloadSlider;

    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;

    public bool isPaused;

    public GameObject playerDamageScreen;
    public TMP_Text gameGoalCountText;
    public TMP_Text ammoCur, ammoMax;
    public GameObject checkpointPopup;

    float timeScaleOrig;

    int gameGoalCount;

    public static int enemiesKilled = 0;
    public List<GameData.LevelTimeData> levelTimes = new List<GameData.LevelTimeData>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
    }

    private void Start()
    {
        SaveManager.LoadGame();

        if(SceneManager.GetActiveScene().name == "Level Select")
        {
            StartScreen();
            statePause();
        }
        else
        {
            menuActive = null;
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if(menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if(menuActive == menuPause)
            {
                stateUnpause(); 
            }
        }
        
    }
    public void loadTutorial()
    {
        Debug.Log("tutorial Load");
        SceneManager.LoadScene("Level Tut");
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if(PowerUpText.Instance != null && PowerUpText.Instance.popUpParent != null)
            PowerUpText.Instance.popUpParent.gameObject.SetActive(false);

        buttonFunctions.SaveGame(true);
        Debug.Log("auto pause save");
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (PowerUpText.Instance != null && PowerUpText.Instance.popUpParent != null)
            PowerUpText.Instance.popUpParent.gameObject.SetActive(true);

        menuActive.SetActive(false);
        menuActive = null;
    }
    
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if(gameGoalCount <= 0)
        {
            if (StopWatch.instance != null)
            {
                StopWatch.instance.StopStopwatch();
                StopWatch.instance.SaveTimeToSaveManager();
            }
            statePause();
            Debug.Log("Rewards Manager called");
            if(SceneManager.GetActiveScene().buildIndex == finalLevelIndex)
            {
                UnlockHardMode();
                endingCutscene.gameObject.SetActive(true);
            }

            else
                RewardsManager.instance.WinRewards();
        }
    }

    public void UnlockHardMode()
    {
        Debug.Log("Hard Mode unlocked");

        GameData data = SaveManager.LoadGame();
        if(data == null)
        {
            data = new GameData();
        }

        data.HardModeUnlocked = true;
        SaveManager.SaveGame(data); 
        
    }

    public void youLose()
    {
        statePause();
        RewardsManager.instance.LossRewards();
        ResetKillCount();
    }

    public void StartScreen()
    {
        menuActive = StartMenu;
        menuActive.SetActive(true);

        GameData data = SaveManager.LoadGame();
        if(data != null && data.HardModeUnlocked)
        {
            hardModeButton.SetActive(true);
        }
        else
        {
            hardModeButton.SetActive(false);
        }
    }

    public void StartScreenMenu()
    {
        menuActive.SetActive(false);
        menuActive = StartMenu;
        menuActive.SetActive(true);
    }

    public void OptionScreen()
    {
        menuActive.SetActive(false);
        menuActive = OptionsMenu;
        menuActive.SetActive(true);
    }

    public void SoulShop()
    {
        menuActive.SetActive(false);
        menuActive = SoulShopMenu;
        menuActive.SetActive(true);
    }

    public void levelSelect()
    {
        menuActive.SetActive(false);
        menuActive = menuLevelSelect;
        menuActive.SetActive(true);
    }

    public void weaponSelect()
    {
        menuActive.SetActive(false);
        menuActive = menuWeaponSelect;
        menuActive.SetActive(true);
    }

    public void showEnemyAuraOverlay(bool dotON)
    {
        if (dotON)
            EnemyAuraScreenOverlay.SetActive(true);
        else
            EnemyAuraScreenOverlay.SetActive(false);
    }



    public void showLavaOverlay(bool dotON)
    {
        if (dotON)
            LavaScreenOverlay.SetActive(true);
        else
            LavaScreenOverlay.SetActive(false);
    }

    public void backButton()
    {
        menuActive.SetActive(false);
        StartScreen();
    }

    public void ResetKillCount()
  {
       enemiesKilled = 0;
  }
}
