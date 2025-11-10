using UnityEngine;

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
        var hits = Physics.OverlapSphere(position, radius, hitMask, QueryTriggerInteraction.Collide);

        for (int i = 0; i < hits.Length; i++)
        {
            var idmg = hits[i].GetComponent<IDamage>();
            if (idmg == null) continue;

            var ctx = new DamageContext(
                source: source,
                target: hits[i].gameObject,
                baseHitDamage: baseHit
            );

            idmg.takeDamage(in ctx, effects: null);
        }

        // 2) VFX (optional)
        if (explosionVfx)
            Object.Instantiate(explosionVfx, position, Quaternion.identity);
    }
}
