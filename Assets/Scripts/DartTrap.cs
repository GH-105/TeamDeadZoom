using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class DartTrap : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePos;
    private DartTrap parent;

    private void OnTriggerEnter(Collider other)
    {
        var rot = Quaternion.LookRotation(firePos.transform.right);
        if (other.CompareTag("Player"))
        {
            GameObject newBullet = Instantiate(bullet, firePos.position, rot);
        }
    }
}
