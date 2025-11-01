using UnityEngine;
using System.Collections;
public class Damage : MonoBehaviour
{
    enum damagetype { moving, stationary, DOT, DOT1, homing, thrown }
    [SerializeField] damagetype type;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject projModel;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    [SerializeField] float flightTime;
    [SerializeField] GameObject playerInDot;

    bool isDamaging;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damagetype.moving || type == damagetype.homing || type == damagetype.thrown)
        {
            Destroy(gameObject, destroyTime);

            if (type == damagetype.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
            else if(type == damagetype.thrown)
            {
                rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position - 0.5f * Physics.gravity * (flightTime * flightTime)) / flightTime;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == damagetype.homing)
        {
            rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && (type == damagetype.moving || type == damagetype.stationary || type == damagetype.homing || type == damagetype.thrown))
        {
            dmg.takeDamage(damageAmount);

            if (type == damagetype.homing || type == damagetype.moving || type == damagetype.thrown)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damagetype.DOT)
        {
            gameManager.instance.showPlayerDOTScreen(true);
            if (!isDamaging)
            {
                StartCoroutine(damageOther(dmg));
            }
        }
        if (dmg != null && type == damagetype.DOT1)
        {
            if (!isDamaging)
            {
                StartCoroutine(damageOther(dmg));
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == damagetype.DOT)
        {
            gameManager.instance.showPlayerDOTScreen(false);
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
