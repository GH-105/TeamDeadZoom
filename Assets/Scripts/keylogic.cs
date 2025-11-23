using UnityEngine;
using TMPro;

public class keyLogic : MonoBehaviour
{
    public TMP_Text DisplayKeyAmount;
    public TMP_Text KeyLabel;
    [SerializeField] GameObject key;
    public static int keyCount;
    public Transform interactiveSource;
    public float interactRange = 3f;
    public playerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keyCount++;
            DisplayKeyAmount.text = keyCount.ToString();
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = FindFirstObjectByType<playerController>();
    }

    private void Update()
    {
        if (player.HP <= 0)
        {
            KeyLabel.gameObject.SetActive(false);
            return;
        }
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