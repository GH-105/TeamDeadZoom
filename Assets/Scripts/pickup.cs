using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] gunStats gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();

        if(pickup != null)
        {
            gun.ammoCur = gun.ammoMax;
            pickup.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
