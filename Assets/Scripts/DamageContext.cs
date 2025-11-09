using UnityEngine;

public readonly struct DamageContext
{
    public readonly GameObject source;
    public readonly GameObject target;
    public readonly float baseHitDamage;

    public DamageContext(GameObject source, GameObject target, float baseHitDamage)
    {
        this.source = source;
        this.target = target;
        this.baseHitDamage = baseHitDamage;
    }
}

