using UnityEngine;

public class Iinteraction : MonoBehaviour
{
    [SerializeField] int speedBoost;
    [SerializeField] int jumpCIncrease;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController player = other.GetComponent <playerController> ();
            gunStats gunStatP = other.GetComponent<gunStats> ();
            if (player != null)
            {
                player.Speed += (int)speedBoost; player.JumpCountMax += (int)jumpCIncrease;
                
                
                Destroy(gameObject);
            }
        }
    }
}
