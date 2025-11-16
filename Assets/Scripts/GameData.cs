using UnityEngine;
using System.Collections.Generic;


// everything that gets saved goes in here
[System.Serializable]
public class GameData 
{
    public int souls;
    public int coins;
    public int playerHP;
    public Vector3 checkpointPosition;
    public int dashCount;
    public int jumpCount;
    public float playerSpeed;
    public difficulty dL = difficulty.normal;
    public bool HardModeUnlocked = false;
    public bool HardModeSelected = false;

    [System.Serializable]
    public class GunData
    {
        public int flatDamageMod;
        public float damageMultMod;
        public int addMaxAmmoMod;
        public float totalDamage;
        public float totalDist;
        public float totalRate;
        public float totalAmmo;
    }

    public GunData[] gunData;
    public int currentGunIndex;

    [System.Serializable]
    public class LevelTimeData
    {
        public string levelName;
        public float bestTime;
        public float currentTime;
        public int enemiesKilled;
    }

    public List<LevelTimeData> levelTimes = new List<LevelTimeData>();
    public string lastLevelCompleted;
}
