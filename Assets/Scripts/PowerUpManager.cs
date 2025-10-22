using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;//we will use this to check if the player has any

    public int totalSpeed;
    public int totalJumps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()//needs to be done every scene
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PowerUpAdd(int speedBoost, int jumpCIncrease)
    {
        totalSpeed += speedBoost;   
        totalJumps += jumpCIncrease;
    }
    
    public void ApplyToPlayer(playerController player)
    {
        player.Speed += totalSpeed;
        player.JumpCountMax += totalJumps;
    }
}
