using UnityEngine;
using TMPro;

public interface IRewardDoor
{
    void Interact();
}

public class rewardDoor : MonoBehaviour, IRewardDoor
{
    public TMP_Text keyPrompt;
    public TMP_Text notEnoughPrompt;
    [SerializeField] GameObject doorObject;
    bool playerNear = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            keyPrompt.text = "Press E to open";
            keyPrompt.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            keyPrompt.gameObject.SetActive(false);
            notEnoughPrompt.gameObject.SetActive(false);
        }
    }

    public void Interact()
    {
        if (!playerNear)
            return;

        if (keyLogic.keyCount > 0)
        {
            keyLogic.keyCount--;
            doorObject.SetActive(false);
            keyPrompt.gameObject.SetActive(false);
            notEnoughPrompt.gameObject.SetActive(false);
        }
        else
        {
            notEnoughPrompt.text = "Not enough keys!";
            notEnoughPrompt.gameObject.SetActive(true);
        }
    }
}