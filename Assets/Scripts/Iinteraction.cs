using UnityEngine;

public class Iinteraction : MonoBehaviour
{
    [SerializeField] int speedBoost;
    [SerializeField] int jumpCIncrease;
    [SerializeField] int damageBoost;
    [SerializeField] int distBoost;
    [SerializeField] float rateBoost;
    [SerializeField] int ammoUp;
    
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent <playerController> ();
            gunStats gun = other.GetComponentInChildren<gunStats> ();
            if (player != null)
            {
                player.Speed += (int)speedBoost; player.JumpCountMax += (int)jumpCIncrease; 

                if (gun != null)
                {
                    gun.shootDamage += (int)damageBoost;
                    gun.shootDist += (int)distBoost;
                    gun.shootRate += (int)rateBoost;
                    gun.ammoMax += (int)ammoUp;
                }
                
                PowerUpManager.Instance.PowerUpAdd(speedBoost, jumpCIncrease, damageBoost, distBoost, rateBoost, ammoUp);//record data
                
                Destroy(gameObject);
            }
        }
    }
}
