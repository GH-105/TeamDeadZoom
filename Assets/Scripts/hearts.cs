using UnityEngine;
using UnityEngine.UI;

public class hearts : MonoBehaviour
{

    public int health;
    public int maxHealth;

    public Sprite emptyHeart;
    public Sprite fullHeart;
    public Image[] heart;

    public playerController playerCont;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerCont == null || heart == null || fullHeart == null || emptyHeart == null)
            return;

        health = playerCont.HP;
        maxHealth = playerCont.HPOrig;

        int count = Mathf.Min(heart.Length, maxHealth);


        for (int i = 0; i < count; i++)
        {
            if(heart[i] == null)
                continue;

            heart[i].sprite = (i < health) ? fullHeart : emptyHeart;
            heart[i].enabled = true;
        }

        for (int i = count; i < heart.Length; i++)
        {
            if (heart[i] != null)
                heart[i].enabled = false;
        }
    }
}
