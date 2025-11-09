using System.Collections;
using TMPro;
using UnityEngine;

public class CoinShop : MonoBehaviour
{

    private int coins = Coinlogic.coinCount; //make this = to player coins in the future
    [SerializeField] TMP_Text displayCoinAmount;
    [SerializeField] TMP_Text notEnoughCoins;

    [SerializeField] TMP_Text healCostText;
    [SerializeField] TMP_Text ammoCostText;
    [SerializeField] TMP_Text flatDamageCostText;
    [SerializeField] TMP_Text damageMultCostText;
    
    [SerializeField] int healCount = 5;
    [SerializeField] int costOfHealing = 10;
    [SerializeField] int flatDamage = 2;
    [SerializeField] int costOfFlat = 15;
    [SerializeField] float damageMultAmm = 1.5f;
    [SerializeField] int costOfDMult = 20;
    [SerializeField] float ammoMult = 1.5f;
    [SerializeField] int ammoCost = 10;


    public playerController playerContr;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        displayCoinAmount.text = Coinlogic.coinCount.ToString();

        healCostText.text = " - " + costOfHealing.ToString() + "coins for " + healCount.ToString() + " Hp";
        ammoCostText.text = " - " + ammoCost.ToString() + "coins for " + ammoMult.ToString() + " Ammo";
        flatDamageCostText.text = " - " + costOfFlat.ToString() + "coins for +" + flatDamage.ToString() + " Damage";
        damageMultCostText.text = " - " + costOfDMult.ToString() + "coins for *" + damageMultAmm.ToString() + " Damage";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateCoinDisplay()
    {
        displayCoinAmount.text = Coinlogic.coinCount.ToString();
    }
    public void buyHealth()
    {
        if(Coinlogic.coinCount >= costOfHealing)
        {
            Coinlogic.coinCount -= costOfHealing;
            playerContr.HP += healCount;
            if(playerContr.HP > playerContr.HPOrig)
                playerContr.HP = playerContr.HPOrig;
            print("Healed: you now have: " + playerContr.HP.ToString());
            UpdateCoinDisplay();
            StartCoroutine(displayBought());
        }
        else
        {
            StartCoroutine(displayTooBroke());
        }
    }

    public void buyAmmo()
    {
        if(Coinlogic.coinCount >= ammoCost)
        {
            Coinlogic.coinCount -= ammoCost;

            int gunIndex =playerContr.gunListPos;
            PowerUpManager.Instance.ApplyAmmoBonus(gunIndex, Mathf.CeilToInt(PowerUpManager.Instance.GetMaxAmmo(gunIndex) * ammoMult));
            UpdateCoinDisplay();
            StartCoroutine(displayBought());
        }
        else
        {
            StartCoroutine(displayTooBroke());
        }
    }

    public void buyFlatDamage()
    {
        if(Coinlogic.coinCount >= costOfFlat)
        {
            Coinlogic.coinCount -= costOfFlat;

            PowerUpManager.Instance.ApplyFlatDamage(playerContr.gunListPos, flatDamage);
            UpdateCoinDisplay();
            StartCoroutine(displayBought());
        }
        else
        {
            StartCoroutine(displayTooBroke());
        }
    }

    public void buyDamageMult()
    {
        if(Coinlogic.coinCount >= costOfDMult)
        {
            Coinlogic.coinCount -= costOfDMult;
            int gunIndex = playerContr.gunListPos;

            PowerUpManager.Instance.ApplyDamageMultiplier(gunIndex, damageMultAmm);
            UpdateCoinDisplay();
            StartCoroutine(displayBought());
        }
        else
        {
            StartCoroutine(displayTooBroke());
        }
    }
    IEnumerator displayTooBroke()
    {
        notEnoughCoins.gameObject.SetActive(true);
        notEnoughCoins.text = "Not Enough Coins";
        yield return new WaitForSeconds(1f);
        notEnoughCoins.gameObject.SetActive(false);
    }

    IEnumerator displayBought()
    {
        notEnoughCoins.gameObject.SetActive(true);
        notEnoughCoins.text = "you have " + Coinlogic.coinCount.ToString() + " left.";
        yield return new WaitForSeconds(1f);
        notEnoughCoins.gameObject.SetActive(false);
    }
}
