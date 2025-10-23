using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuRoomComp;
    [SerializeField] GameObject menuLevelSelect;
    [SerializeField] GameObject menuWeaponSelect;
    [SerializeField] GameObject playerDOTScreen;

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
        Debug.Log($"GM Awake: {GetInstanceID()} in scene {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
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
            levelSelect();
            statePause();
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
        }
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void levelSelect()
    {
        menuActive = menuLevelSelect;
        menuActive.SetActive(true);
    }

    public void weaponSelect()
    {
        menuActive.SetActive(false);
        menuActive = menuWeaponSelect;
        menuActive.SetActive(true);
    }

    public void showPlayerDOTScreen(bool dotON)
    {
        if (dotON)
            playerDOTScreen.SetActive(true);
        else
            playerDOTScreen.SetActive(false);
    }

    public void showRoomComplete()
    {
        
    }
}
