using UnityEngine;

public interface IStatusDamageReceiver
{
    void ApplyDot(float amount, DamageEffects def, GameObject source);
}