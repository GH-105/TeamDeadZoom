using System;
using TMPro;
using UnityEngine;

public class levelSelDisplay : MonoBehaviour
{
    [System.Serializable] //adds ways to connect in unity
    public class levelUI
    {
        public string sceneName;
        public TMP_Text bestTimeText;
    }

    public levelUI[] levels;

    void Start()
    {
        UpdateAllBestTime();    
    }

    // Update is called once per frame
    void UpdateAllBestTime()
    {
        foreach (var level in levels)
        {
            if (level.bestTimeText == null) continue; //if empty skip 

            string key = "BestTime_" + level.sceneName;
            float bestTime = PlayerPrefs.GetFloat(key, float.MaxValue);

            if (bestTime == float.MaxValue)
                level.bestTimeText.text = "--:--:--";
            else
                level.bestTimeText.text = TimeSpan.FromSeconds(bestTime).ToString(@"mm\:ss\:ff");
        }
    }
}
