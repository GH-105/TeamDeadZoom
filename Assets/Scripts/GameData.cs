using NUnit.Framework; // was this auto added?
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


    [System.Serializable]
    public class GunData
    {
        public int flatDamageMod;
        public float damageMultMod;
        public int addMaxAmmoMod;
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
