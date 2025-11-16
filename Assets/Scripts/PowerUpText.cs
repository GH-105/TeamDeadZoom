using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Rendering;


public class PowerUpText : MonoBehaviour
{
    public static PowerUpText Instance;

    [SerializeField] private TextMeshProUGUI popUpText;
    [SerializeField] private Transform popUpParent;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(string message)
    {
        if (popUpText == null || popUpParent == null)
            return;

        TextMeshProUGUI text = Instantiate(popUpText, popUpParent);
        text.text = message;
        StartCoroutine(FadeAndDestroy(text));
    }

    IEnumerator FadeAndDestroy(TextMeshProUGUI text)
    {
        float duration = 1.5f;
        float time = 0f;
        Color c = text.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            text.color = new Color(c.r, c.g, c.b, 1 - (time / duration));
            text.transform.Translate(Vector3.up * Time.deltaTime * 40);
            yield return null;
        }
        Destroy(text.gameObject);

    }

}
