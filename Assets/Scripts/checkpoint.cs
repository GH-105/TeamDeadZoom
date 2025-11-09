using UnityEngine;
using System.Collections;

public class checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;

    Color colorOrig;

    void Start()
    {
        colorOrig = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerSpawnPos.transform.position != transform.position)
        {
            gameManager.instance.playerSpawnPos.transform.position = transform.position;
            StartCoroutine(feedback());

            GameData data = new GameData
            {
                souls = SoulManagement.souls,
                playerHP = (int)gameManager.instance.playerScript.HP,
                checkpointPosition = transform.position
            };
            SaveManager.SaveGame( data);
            Debug.Log("saved");
        }
    }

    IEnumerator feedback()
    {
        model.material.color = Color.red;
        gameManager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        gameManager.instance.checkpointPopup.SetActive(false);
        model.material.color = colorOrig;
    }
}
