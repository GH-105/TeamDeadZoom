using UnityEngine;

[CreateAssetMenu(fileName = "DamagingAuraEffect", menuName = "Scriptable Objects/DamagingAuraEffect")]
public class DamagingAura : DamageEffects 
{
    public override void OnTick(statusController target, statusController.RuntimeEffect rt)
    {
        if (target.Receiver == null) return;

        if (!target.CompareTag("Player")) return;

        float dmg = rt.baseHitDamage * rt.magnitude;
        if (dmg > 0f) target.Receiver.ApplyDot(dmg, this, rt.source);
    }
}
