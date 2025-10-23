using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class gunModifiers
{
    public int flatDamageMod;
    public float damageMultMod = 1f;
    public float rateMultMod = 1f;
    public int addMaxAmmoMod;
    public int addGunRangeMod;
}

[System.Serializable]
public class gunState
{
    public int ammoCur;
}

[System.Serializable]
public class GunListings
{
    public gunStats baseStats;
    public gunModifiers mods = new gunModifiers();
    public gunState state = new gunState();
}
