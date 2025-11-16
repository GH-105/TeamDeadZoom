using UnityEngine;
public enum difficulty
{
    normal = 0,
    Hard = 1,
}
public static class DifficultyManager
{
    public static difficulty currDif = difficulty.normal;//ensure we start normal    
}
