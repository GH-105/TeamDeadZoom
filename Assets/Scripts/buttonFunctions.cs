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

    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void quit()
    {
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
          
            SceneManager.LoadScene(level);
            gameManager.instance.stateUnpause();
        }
    }

    public void nextLevel()
    {
        SceneManager.LoadScene(nextIndex);
        gameManager.instance.stateUnpause();
    }

    public void loadLevelSelectMenu()
    {
        gameManager.instance.levelSelect();
    }

    public void LoadWeapon(gunStats gun)
    {
        Debug.Log("LoadWeapon clicked");
        SceneManager.LoadScene(levelChosen);
        PowerUpManager.Instance.SetStartingGun(gun);
        gameManager.instance.stateUnpause();
    }

    public void SaveGame()
    {
        GameData data = new GameData
        {
            souls = SoulManagement.souls,
            playerHP = gameManager.instance.playerScript.HP,
            checkpointPosition = gameManager.instance.playerSpawnPos.transform.position
        };

        SaveManager.SaveGame(data);
    }
    public void LoadGame()
    {
        GameData data = SaveManager.LoadGame();
        if(data != null)
        {
            SoulManagement.souls = data.souls;
            gameManager.instance.playerScript.HP = data.playerHP;
            gameManager.instance.playerSpawnPos.transform.position = data.checkpointPosition;
            gameManager.instance.playerScript.spawnPlayer();

            Debug.Log($"Loaded : {data.souls} souls, HP {data.playerHP}, checkpoint {data.checkpointPosition}");
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

    public void LoadTestLevel(int level)
    {
        loadLevel(level);
    }

    public void BackButton()
    {
        gameManager.instance.StartScreen();
    }
}
