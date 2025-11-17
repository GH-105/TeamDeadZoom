using UnityEngine;

public class CreditsRoll : MonoBehaviour
{
    [SerializeField] private RectTransform creditsContent;
    [SerializeField] private float rollSpeed = 50f;

    void Update()
    {
        creditsContent.anchoredPosition += Vector2.up * rollSpeed * Time.unscaledDeltaTime;    
    }
}
