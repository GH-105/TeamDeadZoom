using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoulManagement : MonoBehaviour
{
    public static SoulManagement instance;
    public static int souls;

    public static int dashUpgradeCost = 1;//1
    public static int hpUpgradeCost = 5;//5
    public static int jumpUpgradeCost = 3;//3
    public static int speedUpgradeCost = 3;//3

    public static int jumpCount = 1;
    public static int dashCount = 0;
    public static int maxHp = 15;
    public static int currentHP;
    public static float playerSpeed = 15f;

    [SerializeField] private TMP_Text soulCountText;
    [SerializeField] private TMP_Text dashUpgradeCostText;
    [SerializeField] private TMP_Text hpUpgradeCostText;
    [SerializeField] private TMP_Text jumpUpgradeCostText;
    [SerializeField] private TMP_Text speedUpgradeCostText;

    [SerializeField] private GameObject dashUpgradeButton;
    [SerializeField] private GameObject hpUpgradeButton;
    [SerializeField] private GameObject jumpUpgradeButton;
    [SerializeField] private GameObject speedUpgradeButton;
    [SerializeField] GameObject backButton;

    [SerializeField] public playerController playercont;

    private void OnEnable()
    {
        playerController.OnPlayerReady += HandlePlayerReady;
    }
    private void OnDisable()
    {
        playerController.OnPlayerReady -= HandlePlayerReady;
    }

    private void HandlePlayerReady(playerController player)
    {
        playercont = player;
        ApplyStatsToPlayer(playercont);
        UpdateUI();
    }
    private void Awake()
    {
        instance = this;
        UpdateUI();
    }


    public static void ApplyStatsToPlayer(playerController player)
    {
        if(player == null) return;

        player.maxAirDash = dashCount;
        player.HPOrig = maxHp;
        player.HP = currentHP > 0 ? currentHP : maxHp;
        player.Speed = (int)playerSpeed;
        player.JumpCountMax = jumpCount;

    }

    public static void AddSouls(int amount)
    {
        souls += Mathf.Max(amount, 0);
        if(instance != null)
            instance.UpdateUI();
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

        if (SpendSouls(dashUpgradeCost))
        {
            dashCount++;
            dashUpgradeCost *= 2;

            ApplyStatsToPlayer(playercont);
            UpdateUI();
        }
    }

    public void UpgradeHealth()
    {
        if(SpendSouls(hpUpgradeCost) && playercont != null)
        {
            maxHp += 5;
            currentHP = maxHp;
            hpUpgradeCost *= 2;

            ApplyStatsToPlayer(playercont);
            UpdateUI();
        }
    }

    public void UpgradeSpeed()
    {
        if(SpendSouls(speedUpgradeCost) && playercont != null)
        {
            playerSpeed += 1f;
            speedUpgradeCost *= 2;

            ApplyStatsToPlayer(playercont);
            UpdateUI();
        }
    }

    public void UpgradeJump()
    {
        if (SpendSouls(jumpUpgradeCost) && playercont != null)
        {
            jumpCount++;
            jumpUpgradeCost *= 2;

            ApplyStatsToPlayer(playercont);
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        soulCountText.text = "Souls: " + souls;

        dashUpgradeCostText.text = "Dash Upgrade: " + dashUpgradeCost + " souls";
        hpUpgradeCostText.text = "HP Upgrade: " + hpUpgradeCost + " souls";
        jumpUpgradeCostText.text = "Jump Upgrade: " + jumpUpgradeCost + " souls";
        speedUpgradeCostText.text = "Speed Upgrade: " + speedUpgradeCost + " souls";

        dashUpgradeButton.SetActive(souls >= dashUpgradeCost);
        hpUpgradeButton.SetActive(souls >= hpUpgradeCost);
        jumpUpgradeButton.SetActive(souls >= jumpUpgradeCost);
        speedUpgradeButton.SetActive(souls >= speedUpgradeCost);

        buttonFunctions.SaveGame(false);

    }
    public static void DeleteSave()
    {
        SaveManager.DeleteSave();

        souls = 0;
        dashUpgradeCost = 1;
        hpUpgradeCost = 5;
        jumpUpgradeCost = 3;
        speedUpgradeCost = 3;

        jumpCount = 1;
        dashCount = 0;
        maxHp = 15;
        currentHP = maxHp;
        playerSpeed = 15;

        playerController player = FindAnyObjectByType<playerController>();
        ApplyStatsToPlayer(player);

        SoulManagement ui = FindAnyObjectByType<SoulManagement>();
        ui?.UpdateUI();
    }


}
