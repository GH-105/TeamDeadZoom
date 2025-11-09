using UnityEngine;
using TMPro;

public class keyLogic : MonoBehaviour
{
    public TMP_Text DisplayKeyAmount;
    [SerializeField] GameObject key;
    public static int keyCount;
    public Transform interactiveSource;
    public float interactRange = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keyCount++;
            DisplayKeyAmount.text = keyCount.ToString();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(interactiveSource.position, interactiveSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IRewardDoor interactObj))
                {
                    interactObj.Interact();
                    DisplayKeyAmount.text = keyCount.ToString(); // update display after spending key
                }
            }
        }
    }
}