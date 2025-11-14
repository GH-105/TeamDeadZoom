using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamage, IStatusDamageReceiver
{
    [SerializeField] Animator anim;
    [SerializeField] Renderer body;
    [SerializeField] Renderer head;
    [SerializeField] Renderer jaw;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;

    [SerializeField] float HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTimer;

    [SerializeField] Transform shootPos1;
    [SerializeField] Transform shootPos2;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bullet2;//for bosses
    [SerializeField] GameObject bullet3;//for bosses
    [SerializeField] GameObject SpecialShot; //for bosses
    [SerializeField] float shootRate;
    [SerializeField] int enemyBulletDamage;
    [SerializeField] int enemyBulletSpeed;
    [SerializeField] List<EffectInstance> enemyEffects;
    [SerializeField] Transform player;
    [SerializeField] bool summoner;
    [SerializeField] float bulletDelay = 1f;

    [SerializeField] GameObject floatingTextPrefab;
    statusController status;

    [SerializeField] private bool givesSouls = false;
    [SerializeField] private int soulsToGive = 1;

    Color colorOrigBody;
    Color colorOrigHead;
    Color colorOrigJaw;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    bool playerInRange;
    bool dead;


    Vector3 playerDir;
    Vector3 startingPos;

    public room thisRoom;

    [SerializeField] Slider HpSlider;
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;

    private int bulletIndex = 0;
    private float maxHP;
    private float summonTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrigBody = body.material.color;
        colorOrigHead = head.material.color;
        colorOrigJaw = jaw.material.color;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        if (thisRoom != null) thisRoom.UpdateRoomGoal(1);
        HpSlider.maxValue = HP;
        HpSlider.value = HP;
        status = GetComponent<statusController>();
        if (status == null) Debug.LogError($"{name}: missing statusController");
        else if (status.Receiver == null) Debug.LogError($"{name}: statusController has no IStatusDamageReceiver");
        maxHP = HP;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;
        if (summoner)
        {
            summonTimer += Time.deltaTime;
        }

        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }

        if (playerInRange && !canSeePlayer())
        {
            checkRoam();
        }
        else if(!playerInRange)
        {
            checkRoam();
        }

        if (anim != null)
            anim.SetFloat("Speed", Mathf.Clamp01(agent.velocity.magnitude / Mathf.Max(agent.speed, 0.001f)));

        UpdateEnemyHP();
    }

    void checkRoam()
    {
        if(roamTimer >= roamPauseTimer && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        if (agent != null)
        {
            agent.SetDestination(hit.position);
        }
        
    }

    bool canSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        
        RaycastHit hit;
        if(Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if(angleToPlayer <= FOV && hit.collider.CompareTag ("Player"))
            {
                if (agent != null)
                {
                    agent.SetDestination(gameManager.instance.player.transform.position);
                }
                if(shootTimer > shootRate)
                {
                    shoot();
                }

                if(agent.remainingDistance <= stoppingDistOrig)
                {
                    faceTarget();
                }

                    agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    void shoot()
    {
   
        shootTimer = 0;
        anim.SetTrigger("Shoot");
        
    }

    public void createBullet()
    { 

        List<GameObject> bullets = new List<GameObject>();

        if (bullet != null) bullets.Add(bullet);
        if (bullet2 != null) bullets.Add(bullet2);
        if (bullet3 != null) bullets.Add(bullet3);

        if (bullets.Count == 0) return;

        GameObject bulletToShoot = bullets[bulletIndex];

        if (summoner && (bulletIndex == 0 || bulletIndex == 1))
        {
            if (summonTimer < bulletDelay)
            {
                if (bullet3 != null)
                {
                    if (shootPos2 != null)
                    {
                        SpawnAndInit(bullet3, shootPos1);
                        SpawnAndInit(bullet3, shootPos2);
                    }
                    else
                    {
                        SpawnAndInit(bullet3, shootPos1);
                    }
                }
                return;
            }
        }

        if (summoner) summonTimer = 0;

        
            if (shootPos2 != null)
            {
                SpawnAndInit(bulletToShoot, shootPos1);
                SpawnAndInit(bulletToShoot, shootPos2);
            }
            else
            {
                SpawnAndInit(bulletToShoot, shootPos1);
            }

            bulletIndex = (bulletIndex + 1) % bullets.Count;

            if (SpecialShot != null && (HP <= maxHP / 2))
            {
                if (shootPos2 != null)
                {
                SpawnAndInit(SpecialShot, shootPos1);
                SpawnAndInit(SpecialShot, shootPos2);
            }
                else
                {
                    SpawnAndInit(SpecialShot, shootPos1);
                }
            }
        
    }

    public void takeDamage(in DamageContext context, IReadOnlyList<EffectInstance> effects)
    {
        if (status != null && effects != null)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                var e = effects[i];

                float mag = ScaleMagnitude(e.effect, e.magnitude, in context);
                if (mag <= 0f) continue;
                status.ApplyEffect(e.effect, in context, mag);
            }
        }

        float finalHit = ComputeFinalHit(context.baseHitDamage, context.source);
        ApplyHP(finalHit, context.source);

        if (HP <= 0)
        {
            Die();
        }
    }

    public void ApplyDot(float amount, DamageEffects effect, GameObject source)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Die();
        }
        OnDamaged(amount, source);
    }

    float ScaleMagnitude(DamageEffects def, float magnitude, in DamageContext context)
    {
        return magnitude;
    }

    void OnDamaged(float finalHit, GameObject source)
    {
        int damageInt = (int)finalHit;
        StartCoroutine(flashRed());
        showDamage(damageInt.ToString());
    }

    float ComputeFinalHit(float baseHit, GameObject source)
    {
        return baseHit;
    }

    void ApplyHP(float amount, GameObject source)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Die();
        }
        if(gameObject != null)
            OnDamaged(amount, source);
    }

    void showDamage(string text)
    {
        if (!floatingTextPrefab) return;

        GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        prefab.GetComponentInChildren<TextMeshPro>().text = text;

        prefab.transform.LookAt(Camera.main.transform);
        prefab.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);

    }

    IEnumerator flashRed()
    {
        body.material.color = Color.red;
        head.material.color = Color.red;
        jaw.material.color = Color.red; 
        yield return new WaitForSeconds(0.1f);
        body.material.color = colorOrigBody;
        head.material.color = colorOrigHead;
        jaw.material.color = colorOrigJaw;
    }

    public void UpdateEnemyHP()
    {
        if(HpSlider != null)
        { 
            HpSlider.value = HP; 
            HpSlider.transform.LookAt(Camera.main.transform);
            HpSlider.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
            HpSlider.transform.position = target.position + offset;
        }
    }

    private void SpawnAndInit(GameObject bullet, Transform spawn)
    {
        var go = Instantiate(bullet, spawn.position, transform.rotation);

        var dmg = go.GetComponent<Damage>();
        if (dmg != null)
        {
            dmg.Init(
                shooter: gameObject,
                dmg: enemyBulletDamage,
                speed: enemyBulletSpeed,
                effects: enemyEffects
                );

            if (dmg.damageEffects != null)
            {
                for (int i = 0; i < dmg.damageEffects.Count; i++)
                {
                    var eff = dmg.damageEffects[i].effect;
                    if (eff is ExplosiveEffect ex)
                    {
                        Vector3 targetPos = gameManager.instance.player.transform.position;
                        dmg.ArmExplosive(targetPos, ex.Radius);
                        break;
                    }
                }
            }
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        var sc = GetComponent<statusController>();
        if (sc != null) sc.ClearAllEffects();

        var colls = GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colls.Length; i++) colls[i].enabled = false;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent) agent.enabled = false;
        if(thisRoom != null)
            thisRoom.UpdateRoomGoal(-1);

        if(givesSouls)
        {
            SoulManagement.AddSouls(soulsToGive);
            buttonFunctions.SaveGame();
            Debug.Log("gave souls");
        }
        Destroy(gameObject);
    }

    public void takeDamage(in DamageContext context, IReadOnlyList<EffectInstance> effects, Vector3 dmgPos)
    {
        throw new System.NotImplementedException();
    }
}
