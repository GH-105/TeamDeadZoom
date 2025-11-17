using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEffect", menuName = "Scriptable Objects/PoisonEffect")]
public class PoisonEffect : DamageEffects
{

    public override void OnAggregateTick(statusController target, List<statusController.RuntimeEffect> instance)
    {
        if (target.Receiver == null) return;
        

        float total = 0f;
        for (int i = 0; i < instance.Count; i++)
        {
            var rt = instance[i];
            total += rt.baseHitDamage * rt.magnitude * rt.stacks;
        }

        if (total > 0f)
        {
           
            target.Receiver.ApplyDot(total, this, instance[0].source);
        }
            
    }
}
