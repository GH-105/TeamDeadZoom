using UnityEngine;
using UnityEngine.UI;

public class DamageDirection : MonoBehaviour
{
    [SerializeField] public Transform PlayerObj;
    [SerializeField] public Transform ImagePivot;
    [SerializeField] public Vector3 TakeDmgDirection;
    [SerializeField] public CanvasGroup DmgImageCanvas;
    [SerializeField] public float FadeStart, FadeDur, MaxFade;

    void Start()
    {
        PlayerObj = gameManager.instance.player.transform;
        MaxFade = FadeDur;
    }

    // Update is called once per frame
    void Update()
    {
        if(FadeStart > 0)
        {
            FadeStart -= Time.deltaTime;
        }
        else
        {
            FadeDur -= Time.deltaTime;
            DmgImageCanvas.alpha = FadeDur / MaxFade;
            if(FadeDur <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        TakeDmgDirection.y = PlayerObj.position.y;
        Vector3 DmgDirection = (TakeDmgDirection - PlayerObj.position).normalized;
        float DmgAngle = (Vector3.SignedAngle(PlayerObj.forward, DmgDirection, Vector3.up));
        ImagePivot.transform.localEulerAngles = new Vector3(0, 0, -DmgAngle);
    }
}
