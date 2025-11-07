using UnityEngine;

public class CoinShop : MonoBehaviour
{

    private int coins = 1000; //make this = to player coins in the future
    
    [SerializeField] int healCount = 5;
    [SerializeField] int costOfHealing = 10;

    public playerController playerContr;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buyHealth()
    {
        if(coins >= costOfHealing)
        {
            coins -= costOfHealing;
            playerContr.HP += healCount;
            print("Healed: you now have: " + playerContr.HP.ToString());
        }
        else
        {
            print("not enough coins");
        }
    }
}
