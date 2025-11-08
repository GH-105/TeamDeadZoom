using TMPro;
using UnityEngine;

public class Coinlogic : MonoBehaviour
{
    public TMP_Text DisplayCoinAmount;
    [SerializeField] int coinVal;
    static int coinCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            coinCount += coinVal;
            DisplayCoinAmount.text = coinCount.ToString();
            Destroy(gameObject);
        }
    }
}
