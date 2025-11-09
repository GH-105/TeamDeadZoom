using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
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

    public gunStats startingGun;

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

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }
    
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if(gameGoalCount <= 0)
        {
            menuActive = menuWin;
            menuActive.SetActive(true);
            statePause();
            
            if(StopWatch.instance != null)
                StopWatch.instance.StopStopwatch();
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void StartScreen()
    {
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
}
