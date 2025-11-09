using UnityEngine;
using System.Collections;
using UnityEngine.SubsystemsImplementation;
public class Traps : MonoBehaviour
{
    [SerializeField] GameObject pressurePlate;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject trapDoor;
    [SerializeField] Transform firePOS;
    [SerializeField] float fireSpeed = 10f;

    public enum TrapType
    {
        bullet,
        trapDoor
    }
    public TrapType type;

    private void OnTriggerEnter(Collider other)
    {
        if (type == TrapType.bullet)
        {
            GameObject dart = Instantiate(bullet, firePOS.position, firePOS.rotation);
            dart.GetComponent<Rigidbody>().AddForce(firePOS.forward * fireSpeed, ForceMode.Impulse);
        }
        if (type == TrapType.trapDoor)
        {
            Destroy(pressurePlate);
            Destroy(trapDoor);
        }
    }

}
