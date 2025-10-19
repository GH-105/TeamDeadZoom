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
        health = playerCont.HP;
        maxHealth = playerCont.HPorig;

        for (int i = 0; i < heart.Length; i++)
        {
            if (i < health)
            {
                heart[i].sprite = fullHeart;
            }
            else
            {
                heart[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                heart[i].enabled = true;
            }
            else
            {
                heart[i].enabled = false;
            }
        }
    }
}
