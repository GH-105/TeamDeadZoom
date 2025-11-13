using UnityEngine;
using UnityEngine.UI;

public class LowHpFlash : MonoBehaviour
{
    public Image LowHealthFlash;
    public playerController PlayerHP;

    [SerializeField] public float colorMaxAlpha;
    [SerializeField] public float colorCurrentAlpha;
    [SerializeField] public float pulseSpeed;
    [SerializeField] public float LowHpThreshold;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerHP == null || LowHealthFlash == null)
        {
            return;
        }

        //PlayerHP = gameManager.instance.playerScript;
        if(PlayerHP.HP <= LowHpThreshold)
        {
            float hpflashRatio = Mathf.Clamp01(PlayerHP.HP / LowHpThreshold);
            colorCurrentAlpha = Mathf.Lerp(colorMaxAlpha, 0, hpflashRatio);
        }
        else
        {
            colorCurrentAlpha = 0f;
        }
        Color screenColor = LowHealthFlash.color;
        screenColor.a = Mathf.Lerp(screenColor.a, colorCurrentAlpha, Time.deltaTime * pulseSpeed);
        LowHealthFlash.color = screenColor;
    }
}
