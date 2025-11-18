using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "ExplosiveEffect", menuName = "Scriptable Objects/ExplosiveEffect")]
public class ExplosiveEffect : DamageEffects
{
    [SerializeField] private float radius = 4f;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private GameObject explosionVfx;
    [SerializeField] private float baseHit;
    public float Radius => radius;
    public override void OnProjectileImpact(Vector3 position, GameObject source)
    {
        Debug.Log($"[ExplosiveEffect] Using radius: {radius}", this);

        var hits = Physics.OverlapSphere(position, radius, hitMask, QueryTriggerInteraction.Ignore);

        Debug.Log($"[ExplosiveEffect] Hit count: {hits.Length}");

        for (int i = 0; i < hits.Length; i++)
        {
            float d = Vector3.Distance(position, hits[i].transform.position);
            var idmg = hits[i].GetComponent<IDamage>();
            Debug.Log($"  -> hit {hits[i].name} at distance {d}, has IDamage = {idmg != null}");

            if (idmg == null) continue;

            var ctx = new DamageContext(
                source: source,
                target: hits[i].gameObject,
                baseHitDamage: baseHit
            );
            idmg.takeDamage(in ctx, effects: null);
        }

        if (explosionVfx != null)
        {
            var vfx = Object.Instantiate(explosionVfx, position, Quaternion.identity);
            Object.Destroy(vfx, 2f);
        }

    }
}
