using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript1 : MonoBehaviour
{
    [SerializeField] GameObject creditsPanel;

    public PlayableDirector director;
    public string creditsSceneName = "CreditsScene";
    void Start()
    {
        director.Play();
        director.stopped += OnCutsceneFinished;
    }

    private void OnCutsceneFinished(PlayableDirector d)
    {
        gameManager.instance.statePause();
        creditsPanel.SetActive(true);
    }
}
