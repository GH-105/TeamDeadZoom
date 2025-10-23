using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;//we will use this to check if the player has any

    public int totalSpeed;
    public int totalJumps;
    public int totalDamage;
    public int totalDist;
    public float totalRate;
    public int totalAmmo;

    public List<GunListings> gunList;
    public int gunListPos;

    

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()//needs to be done every scene
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogWarning($"[PowerUpManager] Duplicate in scene '{gameObject.scene.name}', destroying this one.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if(gunList == null)
            gunList = new List<GunListings>();

        Debug.Log($"[PowerUpManager] Ready in scene '{gameObject.scene.name}'", this);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public int AddGun(gunStats baseGun)
    {
        var listGun = new GunListings { baseStats = baseGun };
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
    }

    public void ApplyDamageMultiplier(int index, float mult)
    {
        gunList[index].mods.damageMultMod += mult;
    }

    public void ApplyRateMultiplier(int index, float mult)
    {
        gunList[index].mods.rateMultMod += mult;
    }

    public void ApplyAmmoBonus(int index, int amount)
    {
        gunList[index].mods.addMaxAmmoMod += amount;
    }

    public void ApplyRangeBonus(int index, int amount)
    {
        gunList[index].mods.addGunRangeMod += amount;
    }


    public (int damage, float rate, int ammoMax, int range) CalcGunStats(int index)
    {
        var gun = gunList[index];

        int damage = Mathf.RoundToInt((gun.baseStats.shootDamage + gun.mods.flatDamageMod) * (gun.mods.damageMultMod));
        float rate = gun.baseStats.shootRate * gun.mods.rateMultMod;
        int ammoMax = gun.baseStats.ammoMax + gun.mods.addMaxAmmoMod;
        int range = gun.baseStats.shootDist + gun.mods.addMaxAmmoMod;

        return (damage, rate, ammoMax, range);

    }
    
    public void ApplyToPlayer(playerController player)
    {
        player.Speed += totalSpeed;
        player.JumpCountMax += totalJumps;
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
}
