using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] gunStats pistol;
    [SerializeField] gunStats shotgun;
    [SerializeField] gunStats rifle;
    [SerializeField] gunStats SMG;
    [SerializeField] public GameObject nextLevelButton;

    int levelChosen;
    int nextIndex;
    private void Start()
    {
        nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if(SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 4)
            nextLevelButton.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        if(StopWatch.instance != null)
        {
            StopWatch.instance.StopStopwatch();
            StopWatch.instance.SaveTimeToSaveManager();
        }
        SaveGame(true);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus)
        {
            if (StopWatch.instance != null)
            {
                StopWatch.instance.StopStopwatch();
                StopWatch.instance.SaveTimeToSaveManager();
            }
            SaveGame(true);
        }    
        
    }

    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        PowerUpManager.Instance.playerCurrentHP = gameManager.instance.playerScript.levelStartHP; 
        LoadingScreen.instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
        gameManager.instance.stateUnpause();
    }

    public void quit()
    {
        //SaveGame();

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }

    public void respawn()
    {
        gameManager.instance.playerScript.spawnPlayer();
        gameManager.instance.stateUnpause();//everytime a button in menu is pressed
    }

    public void loadLevel(int level)
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex == 0)
        {
            levelChosen = level;
            gameManager.instance.weaponSelect();
        }
        else
        {
            if(sceneIndex == level)
            {
                restart();
            }

            gameManager.instance.stateUnpause();
            SceneManager.LoadScene(level);
        }
        LoadGame();
    }

    public void nextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex != 4)
            LoadingScreen.instance.LoadScene(nextIndex);
        else
            LoadingScreen.instance.LoadScene(1);

        gameManager.instance.stateUnpause();
        LoadGame();
    }

    public void loadLevelSelectMenu()
    {
        gameManager.instance.levelSelect();
    }

    public void LoadWeapon(gunStats gun)
    {
        LoadingScreen.instance.LoadScene(levelChosen);
        PowerUpManager.Instance.SetStartingGun(gun);
        
        gameManager.instance.stateUnpause();
    }

    public static void SaveGame(bool win)
    {
        GameData data = SaveManager.LoadGame() ?? new GameData(); // ?? is useful if left = null go with right

        data.souls = SoulManagement.souls;
        data.dashCount = SoulManagement.dashCount;
        data.jumpCount = SoulManagement.jumpCount;
        data.playerSpeed = SoulManagement.playerSpeed;
        data.playerHP = (int)gameManager.instance.playerScript.HP;
        data.maxHP = SoulManagement.maxHp;
        data.currentHP = SoulManagement.currentHP;
        data.dashUpgradeCost = SoulManagement.dashUpgradeCost;
        data.hpUpgradeCost = SoulManagement.hpUpgradeCost;
        data.jumpUpgradeCost = SoulManagement.jumpUpgradeCost;
        data.speedUpgradeCost = SoulManagement.speedUpgradeCost;

        if (win)
        {
            data.coins = Coinlogic.coinCount;
            data.checkpointPosition = gameManager.instance.playerSpawnPos.transform.position;
            data.gunData = new GameData.GunData[PowerUpManager.Instance.gunList.Count];
            data.currentGunIndex = PowerUpManager.Instance.gunListPos;
            data.dL = DifficultyManager.currDif;


            for (int i = 0; i < PowerUpManager.Instance.gunList.Count; i++)
            {
                var gun = PowerUpManager.Instance.gunList[i];
                data.gunData[i] = new GameData.GunData
                {
                    flatDamageMod = PowerUpManager.Instance.gunList[i].mods.flatDamageMod,
                    damageMultMod = PowerUpManager.Instance.gunList[i].mods.damageMultMod,
                    addMaxAmmoMod = PowerUpManager.Instance.gunList[i].mods.addMaxAmmoMod,

                    totalDamage = gun.baseStats.shootDamage + gun.mods.flatDamageMod,
                    totalRate = gun.baseStats.shootRate + gun.mods.rateMultMod,
                    totalAmmo = gun.baseStats.ammoMax + gun.mods.addMaxAmmoMod,
                    totalDist = gun.baseStats.shootDist + gun.mods.addGunRangeMod
                };
            }
            data.currentGunIndex = PowerUpManager.Instance.gunListPos;
        }
        data.levelTimes = new List<GameData.LevelTimeData>(gameManager.instance.levelTimes);
        data.lastLevelCompleted = SceneManager.GetActiveScene().name;

        SaveManager.SaveGame(data);
    }
    public static void LoadGame()
    {
        GameData data = SaveManager.LoadGame() ?? new GameData();
        SoulManagement.souls = data.souls;
        SoulManagement.dashCount = data.dashCount;
        SoulManagement.jumpCount = data.jumpCount;
        SoulManagement.playerSpeed = data.playerSpeed;
        DifficultyManager.currDif = data.dL;
        SoulManagement.maxHp = data.maxHP;
        SoulManagement.currentHP = data.currentHP;
        SoulManagement.dashUpgradeCost = data.dashUpgradeCost;
        SoulManagement.hpUpgradeCost = data.hpUpgradeCost;
        SoulManagement.jumpUpgradeCost = data.jumpUpgradeCost;
        SoulManagement.speedUpgradeCost = data.speedUpgradeCost;
        gameManager.instance.playerScript.HP = data.playerHP;

        Coinlogic.coinCount = data.coins;
        gameManager.instance.playerSpawnPos.transform.position = data.checkpointPosition;
   
        gameManager.instance.playerScript.spawnPlayer();

        SoulManagement.ApplyStatsToPlayer(gameManager.instance.playerScript);
   
            if (data.gunData != null && data.gunData.Length > 0)
            {
                int gunDataLength = data.gunData.Length;
                int gunListCount = PowerUpManager.Instance.gunList.Count;

                int loopCount = Mathf.Min(gunDataLength, gunListCount);

                for (int i = 0; i < loopCount; i++)
                {
                    var gun = PowerUpManager.Instance.gunList[i];
                    var savedGun = data.gunData[i];

                    PowerUpManager.Instance.gunList[i].mods.flatDamageMod = data.gunData[i].flatDamageMod;
                    PowerUpManager.Instance.gunList[i].mods.damageMultMod = data.gunData[i].damageMultMod;
                    PowerUpManager.Instance.gunList[i].mods.addMaxAmmoMod = data.gunData[i].addMaxAmmoMod;

                    PowerUpManager.Instance.totalDamage = (int)savedGun.totalDamage;
                    PowerUpManager.Instance.totalRate = (int)savedGun.totalRate;
                    PowerUpManager.Instance.totalAmmo = (int)savedGun.totalAmmo;
                    PowerUpManager.Instance.totalDist = (int)savedGun.totalDist;
                }
                PowerUpManager.Instance.gunListPos = data.currentGunIndex;
            }
        //SoulManagement.instance.UpdateUI();
    }
    public void DeleteSave()
    {
            SoulManagement.DeleteSave();
    }

    public void StartGame()
    {
        gameManager.instance.levelSelect();
    }

    public void OpenOptions()
    {
        gameManager.instance.OptionScreen();
    }

    public void OpenSoulShop()
    {
        gameManager.instance.SoulShop();
    }

    public void OpenCoinShop()
    {
        RewardsManager.instance.ShowCoinShop();
    }

    public void LoadTestLevel(int level)
    {
        loadLevel(level);
    }

    public void BackButton()
    {
        gameManager.instance.backButton();
    }

    public void SelectHardMode()
    {
        GameData data = SaveManager.LoadGame() ?? new GameData();
        data.HardModeSelected = enabled;
        SaveManager.SaveGame(data); 

        DifficultyManager.currDif = enabled?difficulty.Hard:difficulty.normal;

    }

    public void MainMenuButton()
    {
        Destroy(PowerUpManager.Instance.gameObject);
        LoadingScreen.instance.LoadScene(0);
        gameManager.instance.stateUnpause();
    }

}
