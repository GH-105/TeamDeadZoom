using UnityEngine;

[CreateAssetMenu(fileName = "ExplosiveEffect", menuName = "Scriptable Objects/ExplosiveEffect")]
public class ExplosiveEffect : DamageEffects
{
    [SerializeField] private float radius = 4f;
    [SerializeField] private LayerMask hitMask;

    public override void OnApplied(statusController target, statusController.RuntimeEffect rt)
    {
        Vector3 center = target.transform.position;

        var hits = Physics.OverlapSphere(center, radius, hitMask);
        for (int i = 0; i < hits.Length; i++)
        {
            var idmg = hits[i].GetComponent<IDamage>();
            if (idmg == null) continue;

            float dmg = rt.baseHitDamage * rt.magnitude;
            var context = new DamageContext(rt.source, hits[i].gameObject, dmg);
            idmg.takeDamage(in context, effects: null);
        }
    }
}
