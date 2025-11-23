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
            DisplayCoinAmount.text = coinCount.ToString();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (player.HP <= 0)
        {
            coinLabel.gameObject.SetActive(false);
            return;
        }
    }
    private void Start()
    {
        player = FindFirstObjectByType<playerController>();
    }
}
