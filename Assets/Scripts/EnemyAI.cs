using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer body;
    [SerializeField] Renderer head;
    [SerializeField] Renderer jaw;
    [SerializeField] NavMeshAgent agent;

    [SerializeField] int HP;

    [SerializeField] Transform shootPos1;
    [SerializeField] Transform shootPos2;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    [SerializeField] GameObject floatingTextPrefab;

    Color colorOrigBody;
    Color colorOrigHead;
    Color colorOrigJaw;

    float shootTimer;

    bool playerInRange;

    public room roomScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrigBody = body.material.color;
        colorOrigHead = head.material.color;
        colorOrigJaw = jaw.material.color;
        roomScript = GetComponentInParent<room>();
        roomScript.UpdateRoomGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        if(playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);

            if(shootTimer > shootRate)
            {
                shoot();
            }    
        }
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
        }
    }

    void shoot()
    {
        shootTimer = 0;
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
        showDamage(amount.ToString());

        if(HP <= 0)
        {
            Destroy(gameObject);
            roomScript.UpdateRoomGoal(-1);
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
}
