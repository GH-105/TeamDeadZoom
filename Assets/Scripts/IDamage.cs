using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    void takeDamage(in DamageContext context, IReadOnlyList<EffectInstance> effects, Vector3 dmgPos);
}