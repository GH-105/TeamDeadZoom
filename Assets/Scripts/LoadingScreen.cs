using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;


public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;

    public GameObject loadingScreen;
    public Slider loadingBar;

    private void Awake()
    {
        if(instance == null)
             instance = this; 
    }

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsyn(sceneId));
    }

    IEnumerator LoadSceneAsyn(int sceneId)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false;
        
        while (operation.progress < 0.9f)
        {
            loadingBar.value = operation.progress;
            yield return null;
        }

        loadingBar.value = 1f;
        yield return new WaitForSeconds(0.25f);
        operation.allowSceneActivation = true;

        if(operation.isDone)
            loadingScreen.SetActive(false);
    }
}
