using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "IncendiaryEffect", menuName = "Scriptable Objects/IncendiaryEffect")]
public class IncendiaryEffect : DamageEffects
{
    public override void OnTick(statusController target, statusController.RuntimeEffect rt)
    {
        if (target.Receiver == null) return;

        float dmg = rt.baseHitDamage * rt.magnitude;
        if (dmg > 0f) target.Receiver.ApplyDot(dmg, this, rt.source);
    }
}
