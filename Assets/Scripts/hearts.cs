using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class hearts : MonoBehaviour
{

    [Header("Setup")]
    [SerializeField] RectTransform container;
    [SerializeField] Image heartImage;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;

    public playerController playerCont;
    readonly List<Image> heartList = new();

    private void OnEnable()
    {
        StartCoroutine(InitOncePlayerExists());
    }

    IEnumerator InitOncePlayerExists()
    {
        yield return new WaitUntil(() => gameManager.instance && gameManager.instance.playerScript != null);
        playerCont = gameManager.instance.playerScript;

        BuildHearts((int)playerCont.HPOrig);
        UpdateHearts((int)playerCont.HP);
    }

    void BuildHearts(int maxHealth)
    {
        for (int i = container.childCount - 1; i >= 0; i--) Destroy(container.GetChild(i).gameObject);
        heartList.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            var img = Instantiate(heartImage, container);
            heartList.Add(img);
        }
    }

    public void UpdateHearts(int health)
    {
        for (int i = 0; i < heartList.Count; i++)
        {
            if (!heartList[i]) continue;
            heartList[i].sprite = (i < health) ? fullHeart : emptyHeart;
            heartList[i].enabled = true;
        }
    }
}
