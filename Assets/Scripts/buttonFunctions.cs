using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] gunStats gun;
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

    public void LoadWeapon()
    {
        Debug.Log("LoadWeapon clicked");
        gameManager.instance.setStartingWeapon(gun);
        SceneManager.LoadScene(levelChosen);
        gameManager.instance.stateUnpause();
    }
}
