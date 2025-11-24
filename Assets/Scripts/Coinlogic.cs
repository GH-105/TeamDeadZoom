using TMPro;
using UnityEngine;

public class Coinlogic : MonoBehaviour
{
    public TMP_Text DisplayCoinAmount;
    public TMP_Text coinLabel;
    [SerializeField] int coinVal;
    public static int coinCount;
    public playerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            coinCount += coinVal;
            gameManager.instance.ShowCoinAmount(true);
            gameManager.instance.CoinAmount.text = coinCount.ToString();
            player.PlayCoinAndKeySound();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (player.HP <= 0)
        {
            gameManager.instance.ShowCoinAmount(false);
            return;
        }
    }
    private void Start()
    {
        player = FindFirstObjectByType<playerController>();
    }
}
