using TMPro;
using UnityEditor;
using UnityEngine;

public class SoulManagement : MonoBehaviour
{
    public static int souls = 10;

    public static int dashUpgradeCost = 1;
    public static int hpUpgradeCost = 5;
    public static int jummpUpgradeCost = 3;
    public static int speedUpgradeCost = 5;

    public static int jumpCount = 1;
    public static int dashCount = 0;
    public static int maxHp = 15;
    public static int currentHP;
    public static float playerSpeed = 5f;

    [SerializeField] private TMP_Text soulCountText;
    [SerializeField] private TMP_Text dashUpgradeCostText;
    [SerializeField] private TMP_Text hpUpgradeCostText;
    [SerializeField] private TMP_Text jumpUpgradeCostText;
    [SerializeField] private TMP_Text speedUpgradeCostText;

    [SerializeField] private GameObject dashUpgradeButton;
    [SerializeField] private GameObject hpUpgradeButton;
    [SerializeField] private GameObject jumpUpgradeButton;
    [SerializeField] private GameObject speedUpgradeButton;

    private void Start()
    {
        UpdateUI();

        dashUpgradeCostText.text = "Dash Upgrade: " + dashUpgradeCost + " souls";
        hpUpgradeCostText.text = "HP Upgrade: " + hpUpgradeCost + " souls";
        jumpUpgradeCostText.text = "Jump Upgrade: " + jummpUpgradeCost + " souls";
        speedUpgradeCostText.text = "Speed Upgrade: " + speedUpgradeCost + " souls";
    }

    public static void AddSouls(int amount)
    {
        souls += Mathf.Max(amount, 0);
    }

    public static bool SpendSouls(int amount)
    {
        if(souls >= amount)
        {
            souls -= amount;
            return true;
        }
        return false;
    }
    
    public static int GetSouls()
    {
        return souls;
    }

    public void UpgradeDash()
    {
        Debug.Log("Dash test");
        if(SpendSouls(dashUpgradeCost))
        {
            dashCount++;
            dashUpgradeCost *= 2;
            UpdateUI();
        }
    }

    public void UpgradeHealth()
    {
        if(SpendSouls(hpUpgradeCost))
        {
            maxHp += 5;
            currentHP = maxHp;
            hpUpgradeCost *= 2;
            UpdateUI();
        }
    }

    public void UpgradeSpeed()
    {
        if(SpendSouls(speedUpgradeCost))
        {
            playerSpeed += 1f;
            speedUpgradeCost *= 2;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        Debug.Log("UpdateUI bug test");
        soulCountText.text = "Souls: " + souls;

        dashUpgradeCostText.text = "Dash Upgrade: " + dashUpgradeCost + " souls";
        hpUpgradeCostText.text = "HP Upgrade: " + hpUpgradeCost + " souls";
        jumpUpgradeCostText.text = "Jump Upgrade: " + jummpUpgradeCost + " souls";
        speedUpgradeCostText.text = "Speed Upgrade: " + speedUpgradeCost + " souls";

        dashUpgradeButton.SetActive(souls >= dashUpgradeCost);
        hpUpgradeButton.SetActive(souls >= hpUpgradeCost);
        jumpUpgradeButton.SetActive(souls >= jummpUpgradeCost);
        speedUpgradeButton.SetActive(souls >= speedUpgradeCost);

    }
}
