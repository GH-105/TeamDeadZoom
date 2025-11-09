using UnityEngine;

[CreateAssetMenu(fileName = "LavaEffect", menuName = "Scriptable Objects/LavaEffect")]
public class Lava : DamageEffects
{
    public override void OnTick(statusController target, statusController.RuntimeEffect rt)
    {
        if (target.Receiver == null) return;

        if (!target.CompareTag("Player")) return;

        float dmg = rt.baseHitDamage * rt.magnitude;
        if (dmg > 0f) target.Receiver.ApplyDot(dmg, this, rt.source);
    }
}
