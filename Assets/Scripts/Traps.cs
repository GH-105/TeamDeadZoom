using UnityEngine;
using System.Collections;
using UnityEngine.SubsystemsImplementation;
public class Traps : MonoBehaviour
{
    [SerializeField] GameObject pressurePlate;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject trapDoor;

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

        }
        if (type == TrapType.trapDoor)
        {
            Destroy(pressurePlate);
            Destroy(trapDoor);
        }
    }

}
