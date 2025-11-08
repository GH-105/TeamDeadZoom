using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
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
    public int Hp;

    public List<GunListings> gunList;
    public int gunListPos;

    public bool pstat;
    playerController player;
  

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
            Debug.LogError("[PowerUpManager] AddGun called with NULL baseGun. " +
                           "Did you forget to assign the starting gun?");
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
        RefreshIfCurrent(index);
    }

    public void ApplyDamageMultiplier(int index, float mult)
    {
        gunList[index].mods.damageMultMod += mult;
        RefreshIfCurrent(index);
    }

    public void ApplyRateMultiplier(int index, float mult)
    {
        gunList[index].mods.rateMultMod += mult;
        RefreshIfCurrent(index);
    }

    public void ApplyAmmoBonus(int index, int amount)
    {
        gunList[index].mods.addMaxAmmoMod += amount;
        RefreshIfCurrent(index);
    }

    public void ApplyRangeBonus(int index, int amount)
    {
        gunList[index].mods.addGunRangeMod += amount;
        RefreshIfCurrent(index);
    }
    
    public void ApplySpeedBonus(int Speed)
    {
        totalSpeed += Speed;
        gameManager.instance.playerScript.Speed += totalSpeed;
        pstat = true;
    }

    public void ApplyJumpInc(int Jump)
    {
        totalJumps += Jump;
        gameManager.instance.playerScript.JumpCountMax += totalJumps;
        pstat = true;
    }

    public void ApplyPstats(playerController player)
    {
        gameManager.instance.playerScript.Speed += totalSpeed;
        gameManager.instance.playerScript.JumpCountMax += totalJumps;
    }

    public (int damage, float rate, int range) CalcGunStats(int index)
    {
        var gun = gunList[index];

        int damage = Mathf.RoundToInt((gun.baseStats.shootDamage + gun.mods.flatDamageMod) * (gun.mods.damageMultMod));
        float rate = gun.baseStats.shootRate * gun.mods.rateMultMod;
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
        gunList[index].state.ammoCur = GetMaxAmmo(index);
    }

    public bool ConsumeAmmo(int index)
    {
         if(GetCurrentAmmo(index) < 0)
        {
            return false;
        }
        else
        {
            var currentGunState = gunList[index].state;
            currentGunState.ammoCur--;
            return true;
        }
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
        gameManager.instance.playerScript.HP += Hp;
        if(gameManager.instance.playerScript.HP > gameManager.instance.playerScript.HPOrig)
        {
            gameManager.instance.playerScript.HP = gameManager.instance.playerScript.HPOrig;
        }
    }

}

