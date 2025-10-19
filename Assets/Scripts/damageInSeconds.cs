using UnityEngine;

public class damageInSeconds : MonoBehaviour
{

    [SerializeField] float secondsToDestroy = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, secondsToDestroy);
    }

   
}
