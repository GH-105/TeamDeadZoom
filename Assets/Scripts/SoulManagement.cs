using UnityEngine;

public class SoulManagement : MonoBehaviour
{
    public static int souls = 0;

    public static void AddSouls(int amount)
    {
        souls += Mathf.Max(amount, 0);
    }

    public static bool SpendSouls(int amount)
    {
        if(souls >= amount)
        {
            souls -= amount;
            return true;
        }
        return false;
    }
    
    public static int GetSouls()
    {
        return souls;
    }
}
