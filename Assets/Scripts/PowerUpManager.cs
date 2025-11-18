
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;//we will use this to check if the player has any

    public int totalSpeed;
    public int totalJumps;
    public int totalDamage;
    public int totalDist;
    public float totalRate;
    public int totalAmmo;
    public int totalHPIncrease;
    public int Hp;
    public int totalAirDash;
    public int totaljumpDist;
    public int totalProjPlus;
    public float playerCurrentHP;
    public List<EffectInstance> weaponEffects;
    public bool isReloading = false;
    [SerializeField] public float reloadTime;
    [SerializeField] public AudioClip dryFireSound;

    public List<GunListings> gunList;
    public int gunListPos;

    public bool pstat;
  

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()//needs to be done every scene
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if(gunList == null)
            gunList = new List<GunListings>();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SetStartingGun(gunStats startingGun)
    {
        gunList.Clear();
        AddGun(startingGun);
        gunListPos = 0;
    }

    public int AddGun(gunStats baseGun)
    {
        if (baseGun == null)
        {
            return -1;
        }


        var listGun = new GunListings { baseStats = baseGun };
        EnsureInitialized(listGun);
        listGun.state.ammoCur = GetMaxAmmo(baseGun, listGun.mods);
        gunList.Add(listGun);
        return gunList.Count - 1;
    }

    int GetMaxAmmo(gunStats baseGun, gunModifiers mods)
    {
        return baseGun.ammoMax + mods.addMaxAmmoMod;
    }
    
    public void ApplyFlatDamage(int index, int amount)
    {
        gunList[index].mods.flatDamageMod += amount;
        string gunName = gunList[index].baseStats.name;
        Notify($"+{amount} on {gunName}");
        RefreshIfCurrent(index);
    }

    public void ApplyCHDamage(int index, int amount, int distance)
    {
        gunList[index].mods.flatDamageMod += amount;
        RefreshIfCurrent(index);

        totaljumpDist -= distance;
        gameManager.instance.playerScript.JumpSpeed -= totaljumpDist;
        string gunName = gunList[index].baseStats.name;
        Notify($"+{amount} on {gunName}");
        pstat = true;
    }

    public void ApplyDamageMultiplier(int index, float mult)
    {
        float added = (mult - 1) * 100f;
        gunList[index].mods.damageMultMod += mult;
        string gunName = gunList[index].baseStats.name;
        Notify($"+{added:F0}% Damage on {gunName}");
        RefreshIfCurrent(index);
    }

    public void ApplyRateMultiplier(int index, float mult)
    {
        float added = (mult - 1) * 100f;
        gunList[index].mods.rateMultMod += mult;
        string gunName = gunList[index].baseStats.name;
        Notify($"+{added:F0}% Fire Rate on {gunName}");
        RefreshIfCurrent(index);
    }

    public void ApplyAmmoBonus(int index, int amount)
    {
        gunList[index].mods.addMaxAmmoMod += amount;
        string gunName = gunList[index].baseStats.name;
        Notify($"+{amount} Max Ammo on {gunName}");
        RefreshIfCurrent(index);
    }

    public void ApplyRangeBonus(int index, int amount)
    {
        gunList[index].mods.addGunRangeMod += amount;
        string gunName = gunList[index].baseStats.name;
        Notify($"+{amount} Range on {gunName}");
        RefreshIfCurrent(index);
    }

    public void ApplyWeaponEffect(int index, DamageEffects damageEffect, float deltaMagnitude)
    {
        var list = gunList[index].effects;
        var effectIndex = list.FindIndex(e => e.effect == damageEffect);

        string gunName = gunList[index].baseStats.name;

        if (effectIndex < 0)
        {
            list.Add(new EffectInstance { effect = damageEffect, magnitude = deltaMagnitude });
            Notify($"+{damageEffect.name} applied to {gunName}");
        }
        else
        {
            list[effectIndex].magnitude += deltaMagnitude;
            Notify($"+{deltaMagnitude} applied to {gunName}");
        }
        RefreshIfCurrent(index);
    }

    public void ApplyNumProjBonus(int proj)
    {
        totalProjPlus += proj;
        gameManager.instance.playerScript.numProjectiles += totalProjPlus;
        pstat = true;
    }
    public void ApplySpeedBonus(int Speed)
    {
        totalSpeed += Speed;
        gameManager.instance.playerScript.Speed += totalSpeed;
        Notify($"+{Speed} Speed");
        pstat = true;
    }

    public void ApplyCHSpeedBonus(int Speed, int amount)
    {
        totalSpeed += Speed;
        Hp -= amount;
        gameManager.instance.playerScript.Speed += totalSpeed;
        gameManager.instance.playerScript.HP -= amount;
        Notify($"+{Speed} Speed & -{amount} HP");
        pstat = true;
    }

    public void ApplyJumpInc(int Jump)
    {
        totalJumps += Jump;
        gameManager.instance.playerScript.JumpCountMax += totalJumps;
        Notify($"+{Jump} Jumps");
        pstat = true;
    }

    public void ApplyJumpDistInc(int JumpDist)
    {
        totaljumpDist += JumpDist;
        gameManager.instance.playerScript.JumpSpeed += totaljumpDist;
        Notify("+ jump Distance");
        pstat = true;
    }

    public void ApplyPstats(playerController player)
    {
        gameManager.instance.playerScript.Speed += totalSpeed;
        gameManager.instance.playerScript.JumpCountMax += totalJumps;
        gameManager.instance.playerScript.JumpSpeed += totaljumpDist;
        gameManager.instance.playerScript.numProjectiles += totalProjPlus;
        gameManager.instance.playerScript.HPOrig += totalHPIncrease;
        gameManager.instance.playerScript.HP = playerCurrentHP;

    }
    public (int damage, float rate, int range) CalcGunStats(int index)
    {
        var gun = gunList[index];

        int damage = Mathf.RoundToInt((gun.baseStats.shootDamage + gun.mods.flatDamageMod) * (gun.mods.damageMultMod));
        float rate = gun.baseStats.shootRate / gun.mods.rateMultMod;
        int range = gun.baseStats.shootDist + gun.mods.addGunRangeMod;

        return (damage, rate, range);

    }
    public int GetCurrentAmmo(int index) => gunList[index].state.ammoCur;

    public int GetMaxAmmo(int index)
    {
        var gun = gunList[index];
        return gun.baseStats.ammoMax + gun.mods.addMaxAmmoMod;
    }

    public void ReloadCurrentGun(int index)
    {
        if (!isReloading)
        {
            StartCoroutine(ReloadProcess(index));
        }
    }
    private IEnumerator ReloadProcess(int index)
    {
        isReloading = true;
        float elapsedTime = 0f;
        float reloadDuration = reloadTime;
        if(gameManager.instance.ReloadSlider != null)
        {
            gameManager.instance.ReloadSlider.value = 0f;
        }
        if (gameManager.instance.ReloadSlider != null)
        {
            gameManager.instance.ReloadSlider.gameObject.SetActive(true);
        }

        while(elapsedTime < reloadDuration)
        {
            elapsedTime += Time.deltaTime; 
            if(gameManager.instance.ReloadSlider != null)
            {
                gameManager.instance.ReloadSlider.value = elapsedTime / reloadDuration;
            }
            yield return null;
        }
        gunList[index].state.ammoCur = GetMaxAmmo(index);
        
        if (gameManager.instance != null && gameManager.instance.playerScript != null)
        {
            gameManager.instance.playerScript.updatePlayerUI();
        }

        if(gameManager.instance.ReloadSlider != null)
        {
            gameManager.instance.ReloadSlider.value = 1f;
            gameManager.instance.ReloadSlider.gameObject.SetActive(false);
        }

        isReloading = false;
    }

    public bool ConsumeAmmo(int index)
    {
         if(GetCurrentAmmo(index) <= 0)
         {
            return false;
         }
         var currentGunState = gunList[index].state;
         currentGunState.ammoCur--;
         return true;
    }

    private static void EnsureInitialized(GunListings gun)
    {
        if (gun.mods == null) gun.mods = new gunModifiers { damageMultMod = 1f, rateMultMod = 1f };
        if (gun.state == null) gun.state = new gunState();
    }
    
    private void RefreshIfCurrent(int index)
    {
        var player = gameManager.instance?.playerScript;
        if (player != null && player.gunListPos == index)
            player.changeGun();
    }

    public void HpRecovery(int amount)
    {

        Hp += amount;
        gameManager.instance.playerScript.heartsUI.UpdateHearts((int)gameManager.instance.playerScript.HP);
        gameManager.instance.playerScript.HP += Hp;
        if(gameManager.instance.playerScript.HP > gameManager.instance.playerScript.HPOrig)
        {
            gameManager.instance.playerScript.HP = gameManager.instance.playerScript.HPOrig;
        }

        Notify($"+{amount} HP");
    }

    public void maxHPIncrease(int amount)
    {

        Hp += amount;
        gameManager.instance.playerScript.heartsUI.UpdateHearts((int)gameManager.instance.playerScript.HP);
        gameManager.instance.playerScript.HP += Hp;
        if (gameManager.instance.playerScript.HP > gameManager.instance.playerScript.HPOrig)
        {
            gameManager.instance.playerScript.HPOrig = gameManager.instance.playerScript.HP;
        }

        Notify($"+{amount} HP");
    }

    public void Notify(string message)
    {
        if(PowerUpText.Instance != null)
        {
            PowerUpText.Instance.ShowPopup(message);
        }
    }

    public void StorePlayerHP(float hp)
    {
        playerCurrentHP = hp;
        pstat = true;
    }
}

