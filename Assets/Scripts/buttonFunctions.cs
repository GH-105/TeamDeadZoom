using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] gunStats pistol;
    [SerializeField] gunStats shotgun;
    [SerializeField] gunStats rifle;
    [SerializeField] gunStats SMG;

    int levelChosen;
    int nextIndex;
    private void Start()
    {
        nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
    }

    private void OnApplicationQuit()
    {
        Debug.Log("saving on quit");
        SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if(pauseStatus)
        {
            SaveGame();
            Debug.Log("auto save on pause");
        }    
        
    }

    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
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
    }

    public void nextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex != 4)
            LoadingScreen.instance.LoadScene(nextIndex);
        else
            LoadingScreen.instance.LoadScene(1);

        gameManager.instance.stateUnpause();
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

    public static void SaveGame()
    {
        GameData data = new GameData
        {
            souls = SoulManagement.souls,
            coins = Coinlogic.coinCount,
            playerHP = (int)gameManager.instance.playerScript.HP,
            checkpointPosition = gameManager.instance.playerSpawnPos.transform.position,
            dashCount = SoulManagement.dashCount,
            jumpCount = SoulManagement.jumpCount,
            playerSpeed = SoulManagement.playerSpeed

        };
        data.gunData = new GameData.GunData[PowerUpManager.Instance.gunList.Count];
        data.currentGunIndex = PowerUpManager.Instance.gunListPos;

        for (int i = 0; i < PowerUpManager.Instance.gunList.Count; i++)
        {
            data.gunData[i] = new GameData.GunData
            {
                flatDamageMod = PowerUpManager.Instance.gunList[i].mods.flatDamageMod,
                damageMultMod = PowerUpManager.Instance.gunList[i].mods.damageMultMod,
                addMaxAmmoMod = PowerUpManager.Instance.gunList[i].mods.addMaxAmmoMod
            };
        }
        data.levelTimes = new List<GameData.LevelTimeData>(gameManager.instance.levelTimes);
        data.lastLevelCompleted = SceneManager.GetActiveScene().name;

        SaveManager.SaveGame(data);
    }
    public static void LoadGame()
    {
        GameData data = SaveManager.LoadGame();
        
            if (data != null)
            {
                SoulManagement.souls = data.souls;
                Coinlogic.coinCount = data.coins;
                gameManager.instance.playerScript.HP = data.playerHP;
                gameManager.instance.playerSpawnPos.transform.position = data.checkpointPosition;
                gameManager.instance.playerScript.spawnPlayer();

                SoulManagement.dashCount = data.dashCount;
                SoulManagement.jumpCount = data.jumpCount;
                SoulManagement.playerSpeed = data.playerSpeed;

                Debug.Log($"Loaded : {data.souls} souls, HP {data.playerHP}, checkpoint {data.checkpointPosition}");

                int gunDataLength = data.gunData.Length;
                int gunListCount = PowerUpManager.Instance.gunList.Count;

                int loopCount = Mathf.Min(gunDataLength, gunListCount);    

                for (int i = 0; i < loopCount; i++)
                {
                    PowerUpManager.Instance.gunList[i].mods.flatDamageMod = data.gunData[i].flatDamageMod;
                    PowerUpManager.Instance.gunList[i].mods.damageMultMod = data.gunData[i].damageMultMod;
                    PowerUpManager.Instance.gunList[i].mods.addMaxAmmoMod = data.gunData[i].addMaxAmmoMod;
                }
            }
    }
    public void DeleteSave()
    {

        SaveManager.DeleteSave();
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
   
}
