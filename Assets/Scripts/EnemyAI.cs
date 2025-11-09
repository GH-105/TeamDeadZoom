using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Animator anim;
    [SerializeField] Renderer body;
    [SerializeField] Renderer head;
    [SerializeField] Renderer jaw;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTimer;

    [SerializeField] Transform shootPos1;
    [SerializeField] Transform shootPos2;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] GameObject floatingTextPrefab;

    Color colorOrigBody;
    Color colorOrigHead;
    Color colorOrigJaw;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    bool playerInRange;
    bool isMoving;

    Vector3 playerDir;
    Vector3 startingPos;

    public room thisRoom;

    [SerializeField] Slider HpSlider;
    [SerializeField] Camera healthcam;
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrigBody = body.material.color;
        colorOrigHead = head.material.color;
        colorOrigJaw = jaw.material.color;
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        thisRoom.UpdateRoomGoal(1);
        healthcam = gameManager.instance.HpCamera;
        HpSlider.maxValue = HP;
        HpSlider.value = HP;
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

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
        agent.SetDestination(hit.position);
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
                agent.SetDestination(gameManager.instance.player.transform.position);

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
        if (shootPos2 != null)
        {
            Instantiate(bullet, shootPos1.position, transform.rotation);
            Instantiate(bullet, shootPos2.position, transform.rotation);
        }
        else
        {
            Instantiate(bullet, shootPos1.position, transform.rotation);
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        UpdateEnemyHP();
        agent.SetDestination(gameManager.instance.player.transform.position);
        showDamage(amount.ToString());

        if(HP <= 0)
        {
            Destroy(gameObject);
            gameManager.enemiesKilled++;
            thisRoom.UpdateRoomGoal(-1);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    void showDamage(string text)
    {
        if(floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMeshPro>().text = text;

            prefab.transform.LookAt(Camera.main.transform);
            prefab.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
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
        if(HpSlider != null && healthcam != null)
        { 
            HpSlider.value = HP; 
            HpSlider.transform.LookAt(healthcam.transform);
            HpSlider.transform.rotation = Quaternion.LookRotation(HpSlider.transform.position - healthcam.transform.position);
            HpSlider.transform.position = target.position + offset;
        }
    }

}
