using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject quitButtonMenu;
    [SerializeField] private GameObject quitButtonPause;
    private void Awake()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        if (quitButtonMenu != null)
        {
            quitButtonMenu.SetActive(false);
        }
        if (quitButtonPause != null)
        {
            quitButtonMenu.SetActive(false);
        }
        #endif
    }
}
