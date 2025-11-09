using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Damage : MonoBehaviour
{
    enum MovementType { moving, stationary, homing, thrown, Zone }
    enum ZoneType { lava, enemyAura, NotZone }
    [SerializeField] MovementType moveType;
    [SerializeField] ZoneType zoneType;
    [SerializeField] private DamageEffects zoneEffect;
    [SerializeField] private float zoneMagnitude = 1f;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject projModel;
    [SerializeField] public GameObject shooter;

    [SerializeField] public int damageAmount;
    [SerializeField] public float damageRate;
    [SerializeField] public int speed;
    [SerializeField] public int destroyTime;

    [SerializeField] float flightTime;
    [SerializeField] GameObject playerInDot;

    public List<EffectInstance> damageEffects;
    private Coroutine zoneTickRoutine;
    private statusController zoneTarget;
    private ZoneType? activeZone;
    public bool InEnemyAura => activeZone == ZoneType.enemyAura && zoneTickRoutine != null;
    public bool InLava => activeZone == ZoneType.lava && zoneTickRoutine != null;
    public bool _initialized;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
    }
    public void Init(GameObject shooter, int dmg, int speed, IReadOnlyList<EffectInstance> effects)
    {
        this.shooter = shooter;
        this.damageAmount = dmg;
        this.speed = speed;

        this.damageEffects = CloneEffects(effects);
        _initialized = true;

        if (moveType == MovementType.moving && rb != null)
            rb.linearVelocity = transform.forward * speed;
    }

    private static List<EffectInstance> CloneEffects(IReadOnlyList<EffectInstance> src)
    {
        if (src == null) return null;
        var list = new List<EffectInstance>(src.Count);
        for (int i = 0; i < src.Count; i++)
            list.Add(new EffectInstance { effect = src[i].effect, magnitude = src[i].magnitude });
        return list;
    }    

    void OnEnable()
    {
        if (!_initialized)
        {
            damageEffects = (PowerUpManager.Instance != null)
                ? CloneEffects(PowerUpManager.Instance.weaponEffects)
                : new List<EffectInstance>();
        }

        if (moveType == MovementType.moving || moveType == MovementType.homing || moveType == MovementType.thrown)
        {
            Destroy(gameObject, destroyTime);

            if (moveType == MovementType.moving && rb != null)
            {
                rb.linearVelocity = transform.forward * speed;
            }
            else if (moveType == MovementType.thrown && rb != null)
            {
                rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position - 0.5f * Physics.gravity * (flightTime * flightTime)) / flightTime;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moveType == MovementType.homing)
        {
            rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (zoneType == ZoneType.lava || zoneType == ZoneType.enemyAura)
        {

            var sc = other.GetComponent<statusController>();
            if (sc == null || zoneEffect == null) return;

            zoneTarget = sc;
            playerInDot = other.gameObject;
            activeZone = zoneType;

            if (activeZone == ZoneType.lava)
                gameManager.instance.showLavaOverlay(true);

            if (activeZone == ZoneType.enemyAura)
                gameManager.instance.showEnemyAuraOverlay(true);

            if (zoneTickRoutine != null) StopCoroutine(zoneTickRoutine);
            zoneTickRoutine = StartCoroutine(ZoneTicker());
            return;
        }

        if (other.isTrigger) return;

        var target = other.GetComponent<IDamage>();
        if (target == null)
            return;

        var context = new DamageContext(source: shooter, target: other.gameObject, baseHitDamage: damageAmount);

            target.takeDamage(in context, damageEffects);

        if (moveType == MovementType.homing || moveType == MovementType.moving || moveType == MovementType.thrown)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (zoneType == ZoneType.lava || zoneType == ZoneType.enemyAura)
        {
            if (!other.CompareTag("Player")) return;
            if (playerInDot != null && other.gameObject != playerInDot) return;

            if (zoneTickRoutine != null)
            {
                StopCoroutine(zoneTickRoutine);
                zoneTickRoutine = null;
            }

            if (zoneType == ZoneType.enemyAura)
                gameManager.instance.showEnemyAuraOverlay(false);

            if (zoneType == ZoneType.lava)
                gameManager.instance.showLavaOverlay(false);

            zoneTarget = null;
            playerInDot = null;
            activeZone = null;
        }
    }

    private IEnumerator ZoneTicker()
    {
        var rt = new statusController.RuntimeEffect
        {
            source = shooter != null ? shooter : gameObject,
            baseHitDamage = 1f,
            magnitude = zoneMagnitude
        };

        while (zoneTarget != null)
        {
            zoneEffect.OnTick(zoneTarget, rt);
            yield return new WaitForSeconds(zoneEffect.TickInterval);
        }
    }
}
